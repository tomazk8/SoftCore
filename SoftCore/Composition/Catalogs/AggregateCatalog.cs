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

        public override IEnumerable<ComposablePart> Parts
        {
            get
            {
                IEnumerable<ComposablePart> list = catalogs.SelectMany(x => x.Parts);
                return list;
            }
        }
    }
}
