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
            this.ContractName = contractName;
        }
        
        public string ContractName { get; private set; }
    }
}
