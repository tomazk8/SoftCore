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
        private Licence licence;

        public LicensingCatalog(Catalog baseCatalog, Licence licence)
        {
            this.licence = licence;

            filteredParts = baseCatalog.Parts
                .Where(x => FilterFunction(x))
                .ToArray();
        }

        public override IEnumerable<ComposablePart> Parts => filteredParts;

        public bool FilterFunction(ComposablePart part)
        {
            bool isPartLicensed = false;
            string licensedPartName = SoftCoreLicensing.GetLicensedPartName(part);

            // If attribute exists, it must be in the licence in order for it to be added to the catalog.
            if (licensedPartName != null)
                isPartLicensed = licence.GetLicensedPart(licensedPartName) != null;
            else
                isPartLicensed = true;

            return isPartLicensed;
        }
    }
}
