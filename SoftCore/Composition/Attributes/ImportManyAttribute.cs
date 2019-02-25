using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    /// <summary>
    /// A field with this attribute must be of type IEnumerable<> and will receive instance of all the parts having
    /// an export with the same contract defined in the import.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public sealed class ImportManyAttribute : Attribute
    {
        public ImportManyAttribute()
        {
            ContractName = null;
        }
        public ImportManyAttribute(string contractName)
        {
            ContractName = contractName;
        }
        public ImportManyAttribute(Type type)
        {
            ContractName = CompositionTools.GetContractNameFromListType(type);
        }

        public string ContractName { get; private set; }

        /*protected internal override bool MatchesWith(FieldInfo importFieldInfo, string exportContractName)
        {
            string importContractName = this.ContractName;

            if (string.IsNullOrWhiteSpace(importContractName))
            {
                importContractName = AppComposerTools.GetContractNameFromListType(importFieldInfo.FieldType);

                if (importContractName == null)
                    throw new Exception($"Field '{importFieldInfo.Name}' on class '{importFieldInfo.DeclaringType.Name}' must be of type IEnumerable<>.");
            }

            return importContractName == exportContractName;
        }*/
    }
}
