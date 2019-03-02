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
            Contract = null;
        }
        public ImportManyAttribute(Contract contract)
        {
            Contract = contract;
        }
        public ImportManyAttribute(Type type)
        {
            Contract = CompositionTools.GetContractFromListType(type);
        }

        public Contract Contract { get; private set; }
    }
}
