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
        // TODO: optimize by caching licensed parts
        //private List<ComposablePart> licensedParts = new List<ComposablePart>();

        public SoftCoreLicensing(Licence licence)
        {
            if (licence == null)
                throw new ArgumentNullException(nameof(licence));

            this.licence = licence;
        }

        /// <summary>
        /// There are two steps to fully initialize the licensing system. First is to create a catalog using
        /// a method. This catalog will filter out unlicensed parts. The second step is to assign composite
        /// application to this class. This will enable setting the license parameters in the perts that
        /// import them.
        /// </summary>
        /// <param name="compositeApplication"></param>
        public void AssignCompositeApplication(CompositeApplication compositeApplication)
        {
            this.compositeApplication = compositeApplication;

            compositeApplication.SatisfyingImports += CompositeApplication_SatisfyingImports;
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
