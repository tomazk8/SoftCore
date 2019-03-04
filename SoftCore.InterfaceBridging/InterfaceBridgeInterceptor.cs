using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.InterfaceBridging
{
    internal class InterfaceBridgeInterceptor : IInterceptor
    {
        private Type exportType;
        private object exportInstance;

        public InterfaceBridgeInterceptor(Type exportType, object exportInstance)
        {
            if (!exportType.IsInterface)
                throw new ArgumentException("Export type must be an interface.");

            this.exportType = exportType;
            this.exportInstance = exportInstance;
        }

        public void Intercept(IInvocation invocation)
        {
            // A call is made for an imported interface. This call needs to be bridged
            // to the exported instance using the export interface having the same
            // method with same parameters.
            var exportMethod = Tools.GetMethodToInvoke(invocation.Method, exportType);
            invocation.ReturnValue = exportMethod.Invoke(exportInstance, invocation.Arguments);
        }
    }
}
