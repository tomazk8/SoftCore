using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace SoftCore.InterfaceBridging
{
    public class CompositeApplicationInterfaceBridging
    {
        private readonly CompositeApplication compositeApplication;
        private bool isInterfaceBridgingEnabled = false;
        private ProxyGenerator proxyGenerator;

        public CompositeApplicationInterfaceBridging(CompositeApplication compositeApplication)
        {
            this.compositeApplication = compositeApplication;
            proxyGenerator = new ProxyGenerator();

            compositeApplication.SatisfyingImport += CompositeApplication_SatisfyingImport;
        }

        private void CompositeApplication_SatisfyingImport(object sender, Composition.SatisfyingImportEventArgs e)
        {
            if (!isInterfaceBridgingEnabled)
                return;
            if (e.Instance == null)
                return;
            // Proceed only if field is an interface.
            if (!e.FieldInfo.FieldType.IsInterface)
                return;
            // Proceed only if current value cannot be assigned to a field
            if (e.FieldInfo.FieldType.IsAssignableFrom(e.Instance.GetType()))
                return;

            // If value implements the interface with the same name and same methods, a bridge is created. Note that
            // a field's interface can contain a subset of members in the value interface, but the signatures
            // of matching members must be the same.
            var valueInterfaceInfo = e.Instance.GetType().GetInterface(e.FieldInfo.FieldType.Name);

            // Skip if the value doesn't implement this interface
            if (valueInterfaceInfo == null)
                return;

            // Check if member signatures match
            var fieldInterfaceMembers = e.MemberInvocation.TargetType.GetMembers(BindingFlags.Public | BindingFlags.Instance);

            foreach (var fieldInterfaceMember in fieldInterfaceMembers)
            {
                // Find this member in the values' interface
                var valueInterfaceMember = valueInterfaceInfo.GetMember(fieldInterfaceMember.Name);

                if (valueInterfaceMember == null)
                    throw new Exception($"Interface cannot be bridged because member signatures are not the same. Note that the target interface can have less members.");
            }

            // TODO: trigger event
        }

        public void EnableInterfaceBridging()
        {
            isInterfaceBridgingEnabled = true;
        }
        public void DisableInterfaceBridging()
        {
            isInterfaceBridgingEnabled = false;
        }

        public event EventHandler<InterfaceBridgedEventArgs> InterfaceBridged;
    }

    public class InterfaceBridgedEventArgs : EventArgs
    {
        public Type ExportType { get; private set; }
        public Type ImportType { get; private set; }
    }
}
