using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public abstract class Catalog : IEnumerable<ComposablePart>
    {
        /// <summary>
        /// Gets all of the parts in the catalog
        /// </summary>
        /// <returns></returns>
        protected internal abstract IEnumerable<ComposablePart> Parts { get; }

        /// <summary>
        /// Gets parts that match the contract name.
        /// </summary>
        protected internal abstract IEnumerable<ComposablePart> GetMatchingParts(string contractName);

        protected bool ContractsMatch(string contract1, string contract2)
        {
            return contract1 == contract2;
        }

        public IEnumerator<ComposablePart> GetEnumerator()
        {
            return Parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Parts.GetEnumerator();
        }
    }
}
