using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Licensing.UnitTests
{
    public class LicensingTests
    {
        [Test]
        public void TestLicensingWithLicence()
        {
            Licence licence = new Licence(new List<LicensedPart>()
            {
                new LicensedPart("ClassB")
            });

            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB),
                typeof(ClassC));

            LicensingCatalog licensingCatalog = new LicensingCatalog(catalog, licence);

            Assert.IsTrue(catalog.Parts.Any(x => x.PartType == typeof(ClassB)));
            // Filter catalog must contain the licensed part because of the license
            Assert.IsTrue(licensingCatalog.Parts.Any(x => x.PartType == typeof(ClassB)));

            CompositeApplication compositeApplication = new CompositeApplication(licensingCatalog);
            ClassA classA = compositeApplication.GetExportedValue<ClassA>();

            // ClassB must not be null as it is licensed.
            Assert.IsNotNull(classA.ClassB);
        }

        [Test]
        public void TestLicensingWithoutLicence()
        {
            Licence licence = new Licence(new List<LicensedPart>()
            {
            });

            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB),
                typeof(ClassC));

            LicensingCatalog licensingCatalog = new LicensingCatalog(catalog, licence);

            Assert.IsTrue(catalog.Parts.Any(x => x.PartType == typeof(ClassB)));
            // ClassB must not be presend because of the missing part in the licence
            Assert.IsFalse(licensingCatalog.Parts.Any(x => x.PartType == typeof(ClassB)));

            CompositeApplication compositeApplication = new CompositeApplication(licensingCatalog);

            ClassA classA = compositeApplication.GetExportedValue<ClassA>();

            // ClassB must be null in this case
            Assert.IsNull(classA.ClassB);
        }

        [Test]
        public void TestLicenceParameter()
        {
            LicenceParams licenceParams = new LicenceParams
            {
                MaxInstances = 123,
                Name = "Test"
            };

            Licence licence = new Licence(new List<LicensedPart>()
            {
                new LicensedPart("ClassB", JObject.FromObject(licenceParams))
            });

            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB),
                typeof(ClassC));

            LicensingCatalog licensingCatalog = new LicensingCatalog(catalog, licence);

            CompositeApplication compositeApplication = new CompositeApplication(licensingCatalog);
            SoftCoreLicensing softCoreLicensing = new SoftCoreLicensing(compositeApplication, licence);

            ClassA classA = compositeApplication.GetExportedValue<ClassA>();
            Assert.IsNotNull(classA.ClassB.LicenceParams);
            Assert.AreEqual(classA.ClassB.LicenceParams.MaxInstances, 123);
            Assert.AreEqual(classA.ClassB.LicenceParams.Name, "Test");
        }

        #region Classes
        [Export]
        public class ClassA
        {
            // Since ClassB can be missing it must be optional
            [Import(isOptional: true)]
            private ClassB classB;

            private ClassA()
            {
            }

            public ClassB ClassB => classB;
        }

        [Export]
        [LicensedPart("ClassB")]
        public class ClassB
        {
            [Import]
            private ClassC classC;
            [ImportLicenceParameters]
            private LicenceParams licenceParams;

            private ClassB()
            {
            }

            public ClassC ClassC => classC;
            public LicenceParams LicenceParams => licenceParams;
        }

        [Export]
        public class ClassC
        {
            private ClassC()
            {
            }
        }

        public class LicenceParams
        {
            public int MaxInstances { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }
}
