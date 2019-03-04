using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.InterfaceBridging
{
    internal static class Tools
    {
        public static MethodInfo GetMethodToInvoke(MethodInfo calledMethodOnImportInterface, Type exportType)
        {
            var exportMethods = exportType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name == calledMethodOnImportInterface.Name);

            foreach (var exportMethod in exportMethods)
            {
                var exportMethodParameters = exportMethod.GetParameters();
                var importParameters = calledMethodOnImportInterface.GetParameters();

                // Find the method on the export that matches the invoked method on the import.
                if (AreParametersTheSame(exportMethodParameters, importParameters))
                    return exportMethod;
            }

            throw new Exception("Unable to find a method to invoke on an export interface.");
        }

        public static void CheckInterfaceSignatures(Type importType, Type exportType)
        {
            var exportMethods = exportType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var importMethods = importType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var importMethod in importMethods)
            {
                var importParameters = importMethod.GetParameters();
                bool matchFound = false;

                foreach (var exportMethod in exportMethods)
                {
                    if (importMethod.Name == exportMethod.Name)
                    {
                        var exportMethodParameters = exportMethod.GetParameters();

                        // Find the method on the export that matches the invoked method on the import.
                        if (AreParametersTheSame(exportMethodParameters, importParameters))
                        {
                            matchFound = true;
                            break;
                        }
                    }
                }

                if (!matchFound)
                    throw new Exception("Interface bridging cannot be done because interface signatures don't match");
            }
        }

        private static bool AreParametersTheSame(ParameterInfo[] parameters1, ParameterInfo[] parameters2)
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
