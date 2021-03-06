﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoftCore.Composition
{
    public static class CompositionTools
    {
        /// <summary>
        /// Retrieves contract name from the given type.
        /// </summary>
        public static Contract GetContractFromType(Type type)
        {
            // INFO: IEnumerable<> is not here, because IEnumerable<> itself is a contract type when
            // not importing a list.

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<>)))
                    type = type.GenericTypeArguments.Single();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,,,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,,,,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,,,,,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(ExportFactory<,,,,,,,,>)))
                    type = type.GenericTypeArguments.First();
                else if (type.GetGenericTypeDefinition().Equals(typeof(Lazy<>)))
                    type = type.GenericTypeArguments.Single();
            }

            return new Contract(type.Namespace + "." + type.Name);
        }

        /// <summary>
        /// Retrieves contract name from the given list type.
        /// </summary>
        public static Contract GetContractFromListType(Type type)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                return GetContractFromType(type.GenericTypeArguments.Single());
            }
            else
                return null;
        }

        public static IEnumerable<ComposablePartExport> GetExports(Type partType)
        {
            //var typeInfo = partType.GetTypeInfo();
            List<ComposablePartExport> list = new List<ComposablePartExport>();

            ExportAttribute attribute = null;

            // Find the attribute and on which type it is defined
            Type attributeOwnerType = partType;
            while (attributeOwnerType != null)
            {
                attribute = attributeOwnerType.GetCustomAttribute<ExportAttribute>(false);
                if (attribute != null)
                    break;

                attributeOwnerType = attributeOwnerType.BaseType;
            }

            if (attribute != null)
            {
                // Get exported contracts from attribute
                var contracts = attribute.Contracts.Any() ?
                        attribute.Contracts :
                        new Contract[] { CompositionTools.GetContractFromType(attributeOwnerType) };

                foreach (var contract in contracts)
                {
                    if (!list.Any(x => x.Contract == contract))
                        list.Add(new ComposablePartExport(contract));
                }

                // If there are no contracts in attribute, use the type name as a contract.
                if (!list.Any())
                {
                    Contract contract = CompositionTools.GetContractFromType(attributeOwnerType);
                    list.Add(new ComposablePartExport(contract));
                }
            }

            return list;
        }
        public static IEnumerable<ComposablePartImport> GetImports(Type partType)
        {
            var typeInfo = partType.GetTypeInfo();
            List<ComposablePartImport> list = new List<ComposablePartImport>();

            Type type = partType;

            while (type != null)
            {
                // Find all fields with Import attribute
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        // Import attribute
                        {
                            var attribute = field.GetCustomAttribute<ImportAttribute>();

                            if (attribute != null)
                                list.Add(new ComposablePartImport(attribute, field));
                        }

                        // ImportMany attribute
                        {
                            var attribute = field.GetCustomAttribute<ImportManyAttribute>();

                            if (attribute != null)
                                list.Add(new ComposablePartImport(attribute, field));
                        }
                    }
                }

                type = type.BaseType;
            }

            return list;
        }
    }
}
