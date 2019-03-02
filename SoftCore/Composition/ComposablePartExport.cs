using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    public class ComposablePartExport
    {
        public ComposablePartExport(string contractName)
        {
            this.Contract = new Contract(contractName);
        }
        public ComposablePartExport(Contract contract)
        {
            this.Contract = contract;
        }

        public Contract Contract { get; private set; }
    }
}
