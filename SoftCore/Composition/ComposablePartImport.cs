using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SoftCore.Composition
{
    public class ComposablePartImport
    {
        internal ComposablePartImport(Attribute attribute, Type compositePartType, FieldInfo importingFieldInfo)
        {
            this.FieldInfo = importingFieldInfo;

            ImportAttribute importAttribute = attribute as ImportAttribute;
            ImportManyAttribute importManyAttribute = attribute as ImportManyAttribute;

            string contractName;

            if (importAttribute != null)
                contractName = importAttribute.ContractName;
            else if (importManyAttribute != null)
                contractName = importManyAttribute.ContractName;
            else
                throw new NotImplementedException("Attribute no implemented: " + attribute.GetType().Name);

            // set or create contract name
            if (!string.IsNullOrWhiteSpace(contractName))
                this.ContractName = contractName;
            else if (importManyAttribute != null)
                this.ContractName = CompositionTools.GetContractNameFromListType(importingFieldInfo.FieldType);
            else
                this.ContractName = CompositionTools.GetContractNameFromType(importingFieldInfo.FieldType);

            // Set imports type and method
            TypeInfo typeInfo = importingFieldInfo.FieldType.GetTypeInfo();

            if (importAttribute != null)
            {
                Cardinality = importAttribute.IsOptional ? ImportCardinality.ZeroOrOne : ImportCardinality.ExactlyOne;

                if (typeInfo.IsGenericType &&
                    typeInfo.GetGenericTypeDefinition().Equals(typeof(Lazy<>)) &&
                    typeInfo.GenericTypeArguments != null && typeInfo.GenericTypeArguments.Length == 1)
                {
                    ImportType = typeInfo.GenericTypeArguments.Single();
                    ImportMethod = ImportMethod.Lazy;
                }
                else if (typeInfo.IsGenericType &&
                    typeInfo.GetGenericTypeDefinition().Name.StartsWith("ExportFactory"))
                {
                    ImportType = typeInfo.GenericTypeArguments.First();
                    ImportMethod = ImportMethod.ExportFactory;
                }
                else
                {
                    ImportType = importingFieldInfo.FieldType;
                    ImportMethod = ImportMethod.Direct;
                }
            }
            else if (importManyAttribute != null)
            {
                Cardinality = ImportCardinality.ZeroOrMore;

                if (typeInfo.IsGenericType &&
                    typeInfo.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) &&
                    typeInfo.GenericTypeArguments != null && typeInfo.GenericTypeArguments.Length == 1)
                {
                    ImportType = typeInfo.GenericTypeArguments.Single();
                    ImportMethod = ImportMethod.List;
                }
                else
                    throw new Exception("When ImportMany attribute is used, field type must be IEnumerable<T>.");
            }
        }

        internal bool MatchesWith(ComposablePartExport x)
        {
            return ContractName == x.ContractName;
        }

        public string ContractName { get; private set; }
        /// <summary>
        /// Gets if part is imported directly, using Lazy<T> or using a List<T>.
        /// </summary>
        public ImportMethod ImportMethod { get; private set; }
        /// <summary>
        /// The type being imported. If export factory or lazy import is used, this type is the wrapped generic
        /// type, not the real type of the import.
        /// </summary>
        public Type ImportType { get; private set; }
        public FieldInfo FieldInfo { get; private set; }
        public ImportCardinality Cardinality { get; private set; }
    }

    public enum ImportMethod
    {
        /// <summary>
        /// Class is imported directly
        /// </summary>
        Direct,
        /// <summary>
        /// Class is imported as a Lazy<T> that defers the creation of an instance
        /// </summary>
        Lazy,
        /// <summary>
        /// Export factory is exported
        /// </summary>
        ExportFactory,
        /// <summary>
        /// A list of classes of a specific type is imported
        /// </summary>
        List
    }
}
