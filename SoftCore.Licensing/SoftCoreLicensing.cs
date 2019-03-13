using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.Licensing
{
    public class SoftCoreLicensing
    {
        private Licence licence;
        private CompositeApplication compositeApplication;
        
        public SoftCoreLicensing(CompositeApplication compositeApplication, Licence licence)
        {
            this.compositeApplication = compositeApplication;

            compositeApplication.SatisfyingImports += CompositeApplication_SatisfyingImports;

            if (licence == null)
                throw new ArgumentNullException(nameof(licence));

            this.licence = licence;
        }

        internal static string GetLicensedPartName(ComposablePart part)
        {
            // Check if part has a licensing attribute
            var attribute = part.PartType
                .GetCustomAttributes(true)
                .SingleOrDefault(x => x is LicensedPartAttribute) as LicensedPartAttribute;

            return attribute?.LicensedPartName;
        }

        private void CompositeApplication_SatisfyingImports(object sender, SatisfyingImportsEventArgs e)
        {
            string licensedPartName = GetLicensedPartName(e.Part);

            if (licensedPartName != null)
            {
                var licensedPart = licence.GetLicensedPart(licensedPartName);

                if (licensedPart.Parameters != null)
                {
                    // Check if there is a field into which licence parameters must be injected
                    var fields = e.Instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

                    if (fields != null)
                    {
                        foreach (var field in fields)
                        {
                            var attribute = field.GetCustomAttribute<ImportLicenceParametersAttribute>();

                            if (attribute != null && licensedPart != null)
                            {
                                var parameters = licensedPart.Parameters.ToObject(field.FieldType);
                                field.SetValue(e.Instance, parameters);
                            }
                        }
                    }
                }
            }
        }
    }
}
