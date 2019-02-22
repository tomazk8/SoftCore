using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Composition
{
    public class TypeCatalog : Catalog
    {
        public Type[] types;
        public ComposablePart[] parts;

        public TypeCatalog(params Type[] types)
        {
            this.types = types;
            this.parts = types
                .Select(x => new ComposablePart(x))
                .ToArray();
        }

        public IEnumerable<Type> Types
        {
            get { return types; }
        }

        protected internal override IEnumerable<ComposablePart> Parts
        {
            get { return parts; }
        }

        protected internal override IEnumerable<ComposablePart> GetMatchingParts(string contractName)
        {
            var matchingParts = parts.Where(x => x.Exports.Any(e => ContractsMatch(e.ContractName, contractName)));
            return matchingParts;
        }
    }
}
