using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Licensing
{
    public class LicensingCatalog : Catalog
    {
        private IEnumerable<ComposablePart> filteredParts;

        public LicensingCatalog(Catalog baseCatalog, Func<ComposablePart, bool> filterFunction)
        {
            filteredParts = baseCatalog.Parts
                .Where(x => filterFunction(x))
                .ToArray();
        }

        public override IEnumerable<ComposablePart> Parts => filteredParts;

        public bool FilterFunction(ComposablePart part)
        {
            bool isPartLicensed = false;
            string licensedPartName = GetLicensedPartName(part);

            // If attribute exists, it must be in the licence in order for it to be added to the catalog.
            if (licensedPartName != null)
                isPartLicensed = licence.GetLicensedPart(licensedPartName) != null;
            else
                isPartLicensed = true;

            return isPartLicensed;
        }

        private string GetLicensedPartName(ComposablePart part)
        {
            // Check if part has a licensing attribute
            var attribute = part.PartType
                .GetCustomAttributes(true)
                .SingleOrDefault(x => x is LicensedPartAttribute) as LicensedPartAttribute;

            return attribute?.LicensedPartName;
        }
    }
}
