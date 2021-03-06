﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Composition
{
    public class FilterCatalog : Catalog
    {
        private IEnumerable<ComposablePart> filteredParts;

        public FilterCatalog(Catalog baseCatalog, Func<ComposablePart, bool> filterFunction)
        {
            filteredParts = baseCatalog.Parts
                .Where(x => filterFunction(x))
                .ToArray();
        }

        public override IEnumerable<ComposablePart> Parts => filteredParts;
    }
}
