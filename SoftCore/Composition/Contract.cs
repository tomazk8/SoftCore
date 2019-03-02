using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
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
            if (obj == null)
                return false;

            Contract otherContract = obj as Contract;

            // TODO: Is it ok to compare contract names like this?
            return otherContract != null &&
                this.contractName.Equals(otherContract.contractName, StringComparison.OrdinalIgnoreCase);
        }
        public override string ToString()
        {
            return contractName;
        }
        public override int GetHashCode()
        {
            return contractName.GetHashCode();
        }

        public static bool operator ==(Contract contract1, Contract contract2)
        {
            if (ReferenceEquals(contract1, contract2))
                return true;

            if (ReferenceEquals(contract1, null))
                return false;
            if (ReferenceEquals(contract2, null))
                return false;

            bool b = contract1.contractName.Equals(contract2.contractName, StringComparison.OrdinalIgnoreCase);
            return b;
        }
        public static bool operator !=(Contract contract1, Contract contract2)
        {
            bool b = !(contract1 == contract2);
            return b;
        }
    }
}
