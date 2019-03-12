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

            Contract contract;

            if (importAttribute != null)
                contract = importAttribute.Contract;
            else if (importManyAttribute != null)
                contract = importManyAttribute.Contract;
            else
                throw new NotImplementedException("Attribute no implemented: " + attribute.GetType().Name);

            // set or create contract name
            if (contract != null)
                this.Contract = contract;
            else if (importManyAttribute != null)
                this.Contract = CompositionTools.GetContractFromListType(importingFieldInfo.FieldType);
            else
                this.Contract = CompositionTools.GetContractFromType(importingFieldInfo.FieldType);

            // Set imports type and method
            TypeInfo typeInfo = importingFieldInfo.FieldType.GetTypeInfo();

            if (importAttribute != null)
            {
                Cardinality = importAttribute.IsOptional ? ImportCardinality.ZeroOrOne : ImportCardinality.ExactlyOne;
            }
            else if (importManyAttribute != null)
            {
                Cardinality = ImportCardinality.ZeroOrMore;

                // Get the type from the enumerable generic type.
                if (typeInfo.IsGenericType &&
                    typeInfo.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) &&
                    typeInfo.GenericTypeArguments != null && typeInfo.GenericTypeArguments.Length == 1)
                {
                    typeInfo = typeInfo.GenericTypeArguments.Single().GetTypeInfo();
                }
                else
                    throw new NotSupportedException($"When {nameof(ImportManyAttribute)} is used, the field has to be of type IEnumerable<>.");
            }
            else
                throw new NotSupportedException($"Attribute {attribute.GetType().Name} is not yet supported.");

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
                ImportType = typeInfo;
                ImportMethod = ImportMethod.Direct;
            }

            if (this.Contract == null)
                throw new Exception();
        }

        internal bool MatchesWith(ComposablePartExport x)
        {
            return Contract == x.Contract;
        }

        public Contract Contract { get; private set; }
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
        ExportFactory
    }
}
