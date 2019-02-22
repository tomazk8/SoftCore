using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    /// <summary>
    /// Value of the field that has this attribute will be set to the instance of the composable part
    /// having export with the same contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ImportAttribute : Attribute
    {
        public ImportAttribute(bool isOptional = false)
        {
            ContractName = null;
            IsOptional = isOptional;
        }
        public ImportAttribute(string contractName, bool isOptional = false)
        {
            ContractName = contractName;
            IsOptional = isOptional;
        }
        public ImportAttribute(Type type, bool isOptional = false)
        {
            ContractName = CompositionTools.GetContractNameFromType(type);
            IsOptional = isOptional;
        }

        public string ContractName { get; private set; }
        public bool IsOptional { get; private set; }
    }
}
