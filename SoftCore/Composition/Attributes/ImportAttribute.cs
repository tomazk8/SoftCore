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
    public sealed class ImportAttribute : Attribute
    {
        public ImportAttribute(bool isOptional = false)
        {
            Contract = null;
            IsOptional = isOptional;
        }
        public ImportAttribute(string contractName, bool isOptional = false)
        {
            Contract = new Contract(contractName);
            IsOptional = isOptional;
        }
        public ImportAttribute(Type type, bool isOptional = false)
        {
            Contract = CompositionTools.GetContractFromType(type);
            IsOptional = isOptional;
        }

        public Contract Contract { get; private set; }
        public bool IsOptional { get; private set; }
    }
}
