using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public abstract class Catalog : IEnumerable<ComposablePart>
    {
        /// <summary>
        /// This is not simply a list of parts in this catalog but it's a combination of parts here plus
        /// all the parts in the sub-catalogs. Parts can also be filtered or replaced. Therefore this
        /// property returns parts as they are on this level of the tree which is not simply a sum of
        /// all parts. Imagine it as a map-reduce algorithm applied to all parts from sub-catalogs.
        /// </summary>
        public abstract IEnumerable<ComposablePart> Parts { get; }

        /// <summary>
        /// Gets parts that match the contract name.
        /// </summary>
        public abstract IEnumerable<ComposablePart> GetMatchingParts(string contractName);

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
