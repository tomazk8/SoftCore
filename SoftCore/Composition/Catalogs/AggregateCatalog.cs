using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Composition
{
    public class AggregateCatalog : Catalog
    {
        private IEnumerable<Catalog> catalogs;

        public AggregateCatalog(params Catalog[] catalogs)
        {
            this.catalogs = catalogs;
        }

        protected internal override IEnumerable<ComposablePart> Parts
        {
            get
            {
                IEnumerable<ComposablePart> list = Array.Empty<ComposablePart>();

                foreach (var catalog in catalogs)
                {
                    list = list.Concat(catalog.Parts);
                }

                return list;
            }
        }

        protected internal override IEnumerable<ComposablePart> GetMatchingParts(string contractName)
        {
            var matchingParts = Parts.Where(x => x.Exports.Any(e => ContractsMatch(e.ContractName, contractName)));
            return matchingParts;
        }
    }
}
