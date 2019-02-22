using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Testing
{
    /// <summary>
    /// This catalog takes parts from the base catalog and replaces parts with same
    /// contract name with the parts from the replacement part catalog. The
    /// replacement parts must be tagged with <see cref="Catalog" /> ReplacementExport attribute.
    /// </summary>
    public class ReplacementCatalog : Catalog
    {
        protected override IEnumerable<ComposablePart> Parts => throw new NotImplementedException();

        protected override IEnumerable<ComposablePart> GetMatchingParts(string contractName)
        {
            throw new NotImplementedException();
        }
    }
}
