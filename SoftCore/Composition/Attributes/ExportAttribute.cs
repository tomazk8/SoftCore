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
            Contracts = Array.Empty<Contract>();
        }
        public ExportAttribute(params string[] contractNames)
        {
            Contracts = contractNames.Select(x => new Contract(x));
        }
        public ExportAttribute(params Type[] contractTypes)
        {
            Contracts = contractTypes
                .Select(x => CompositionTools.GetContractFromType(x))
                .ToArray();
        }

        public IEnumerable<Contract> Contracts { get; internal set; }
    }
}
