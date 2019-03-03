using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.InterfaceBridging
{
    internal class InterfaceBridgeProxy : IInterceptor
    {
        private ProxyGenerator proxyGenerator;
        private Type importType;
        private Type exportType;
        private object exportInstance;

        public InterfaceBridgeProxy(ProxyGenerator proxyGenerator, Type importType,
            Type exportType, object exportInstance)
        {
            if (!importType.IsInterface)
                throw new ArgumentException("Import type must be an interface.");
            if (!exportType.IsInterface)
                throw new ArgumentException("Export type must be an interface.");

            this.proxyGenerator = proxyGenerator;
            this.importType = importType;
            this.exportType = exportType;
            this.exportInstance = exportInstance;

            proxyGenerator.CreateInterfaceProxyWithoutTarget(importType, this);
        }

        public void Intercept(IInvocation invocation)
        {
            // A call is made for an imported interface. This call needs to be bridged
            // to the exported instance using the export interface having the same
            // method with same parameters.

            var exportMethods = exportType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name == invocation.Method.Name);

            bool methodInvoked = false;

            foreach (var exportMethod in exportMethods)
            {
                var exportMethodParameters = exportMethod.GetParameters();
                var importParameters = invocation.Method.GetParameters();

                // Find the method on the export that matches the invoked method on the import.
                if (AreParametersTheSame(exportMethodParameters, importParameters))
                {
                    exportMethod.Invoke(exportInstance, invocation.Arguments);

                    methodInvoked = true;
                    break;
                }
            }

            if (!methodInvoked)
                throw new Exception("No proper method could be found on an export.");
        }

        private bool AreParametersTheSame(ParameterInfo[] parameters1, ParameterInfo[] parameters2)
        {
            if ((parameters1 == null || parameters1.Length == 0) && (parameters2 == null || parameters2.Length == 0))
                return true;

            if (parameters1 != null && parameters2 != null && parameters1.Length == parameters2.Length)
            {
                for (int i = 0; i < parameters1.Length; i++)
                {
                    if (!parameters1[i].ParameterType.Equals(parameters2[i].ParameterType))
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
