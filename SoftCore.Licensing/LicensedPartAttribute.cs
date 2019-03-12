using System;

namespace SoftCore.Licensing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LicensedPartAttribute : Attribute
    {
        public LicensedPartAttribute(string licensedPartName)
        {
            this.LicensedPartName = licensedPartName;
        }

        public string LicensedPartName { get; private set; }
    }
}
