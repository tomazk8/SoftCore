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

            // If instance implements the interface with the same name and same methods, a bridge is created. Note that
            // a field's interface can contain a subset of members in the instance interface, but the signatures
            // of matching members must be the same.
            var exportInterfaceType = e.Instance.GetType().GetInterface(e.FieldInfo.FieldType.Name);

            // Skip if the value doesn't implement this interface
            if (exportInterfaceType == null)
                return;

            // Check if member signatures match
            Tools.CheckInterfaceSignatures(e.FieldInfo.FieldType, exportInterfaceType);

            InterfaceBridged?.Invoke(this, new InterfaceBridgedEventArgs { ExportType = exportInterfaceType, ImportType = e.FieldInfo.FieldType });

            InterfaceBridgeInterceptor interceptor = new InterfaceBridgeInterceptor(exportInterfaceType, e.Instance);
            object proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget(e.FieldInfo.FieldType, interceptor);
            e.Instance = proxy;
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
        public Type ExportType { get; set; }
        public Type ImportType { get; set; }
    }
}
