using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Licensing
{
    public class Licence
    {
        public Licence(IEnumerable<LicensedPart> licensedParts)
        {
            this.LicensedParts = licensedParts != null ? licensedParts.ToArray() : Array.Empty<LicensedPart>();
        }

        public IEnumerable<LicensedPart> LicensedParts { get; private set; }

        public LicensedPart GetLicensedPart(string licensedPartName)
        {
            return LicensedParts.SingleOrDefault(x => x.LicensedPartName.Equals(licensedPartName, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class LicensedPart
    {
        public LicensedPart(string licensedPartName)
        {
            this.LicensedPartName = licensedPartName;
        }
        public LicensedPart(string licensedPartName, JObject parameters)
        {
            this.LicensedPartName = licensedPartName;
            this.Parameters = parameters;
        }

        public string LicensedPartName { get; private set; }
        public JObject Parameters { get; private set; }
    }
}
