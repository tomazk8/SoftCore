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

    /// <summary>
    /// Represents a contract based on which exports and imports are matched.
    /// </summary>
    public class Contract
    {
        private string contractName;

        public Contract(string contractName)
        {
            if (string.IsNullOrWhiteSpace(contractName))
                throw new Exception("Contract name cannot be empty.");

            this.contractName = contractName;
        }

        public override bool Equals(object obj)
        {
            Contract otherContract = obj as Contract;

            // TODO: Is it ok to compare contract names like this?
            return otherContract != null &&
                this.contractName.Equals(otherContract.contractName, StringComparison.OrdinalIgnoreCase);
        }
        public override string ToString()
        {
            return contractName;
        }

        public static bool operator ==(Contract contract1, Contract contract2)
        {
            return contract1.Equals(contract2);
        }
        public static bool operator !=(Contract contract1, Contract contract2)
        {
            return !contract1.Equals(contract2);
        }
    }
}
