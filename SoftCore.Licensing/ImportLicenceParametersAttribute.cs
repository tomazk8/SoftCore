using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Licensing
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ImportLicenceParametersAttribute : Attribute
    {
    }
}
