using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Composition
{
    /// <summary>
    /// The class with this attribute will be put into the IoC container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ExportAttribute : Attribute
    {
        public ExportAttribute()
        {
            ContractNames = Array.Empty<string>();
        }
        public ExportAttribute(params string[] contractNames)
        {
            ContractNames = contractNames;
        }
        public ExportAttribute(params Type[] contractTypes)
        {
            ContractNames = contractTypes
                .Select(x => CompositionTools.GetContractNameFromType(x))
                .ToArray();
        }

        public IEnumerable<string> ContractNames { get; internal set; }
    }
}
