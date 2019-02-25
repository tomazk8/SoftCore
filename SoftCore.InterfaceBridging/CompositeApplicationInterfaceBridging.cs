using SoftCore.Interception;
using System;
using System.Reflection;

namespace SoftCore.InterfaceBridging
{
    public class CompositeApplicationInterfaceBridging
    {
        private readonly CompositeApplication compositeApplication;
        private readonly CompositeApplicationInterceptor interceptor;
        private bool isInterfaceBridgingEnabled = false;

        public CompositeApplicationInterfaceBridging(CompositeApplication compositeApplication)
        {
            this.compositeApplication = compositeApplication;

            interceptor = new CompositeApplicationInterceptor(compositeApplication);
            interceptor.CallIntercepted += Interceptor_CallIntercepted;
        }

        public void EnableInterfaceBridging()
        {
            isInterfaceBridgingEnabled = true;
        }
        public void DisableInterfaceBridging()
        {
            isInterfaceBridgingEnabled = false;
        }

        private void Interceptor_CallIntercepted(object sender, CallInterceptedEventArgs e)
        {
            if (!isInterfaceBridgingEnabled)
                return;
            if (e.MemberInvocation.ReturnValue == null)
                return;
            // Proceed only if field is an interface.
            if (!e.MemberInvocation.TargetType.IsInterface)
                return;
            // Proceed only if current value cannot be assigned to a field
            if (e.MemberInvocation.TargetType.IsAssignableFrom(e.MemberInvocation.ReturnValue.GetType()))
                return;

            // If value implements the interface with the same name and same methods, a bridge is created. Note that
            // a field's interface can contain a subset of members in the value interface, but the signatures
            // of matching members must be the same.
            var valueInterfaceInfo = e.MemberInvocation.ReturnValue.GetType().GetInterface(e.MemberInvocation.TargetType.Name);

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
        }
    }
}
