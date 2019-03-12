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
        public void TestLicensing()
        {
            Licence licence = new Licence(new List<LicensedPart>()
            {
                new LicensedPart("ClassB")
            });

            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB),
                typeof(ClassC));

            SoftCoreLicensing licensing = new SoftCoreLicensing(licence);
            var filterCatalog = licensing.CreateFilterCatalog(catalog);

            Assert.IsTrue(catalog.Parts.Any(x => x.PartType == typeof(ClassB)));
            // Filter catalog must contain the licensed part because of the license
            Assert.IsTrue(filterCatalog.Parts.Any(x => x.PartType == typeof(ClassB)));

            CompositeApplication compositeApplication = new CompositeApplication(filterCatalog);
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

            SoftCoreLicensing licensing = new SoftCoreLicensing(licence);
            var filterCatalog = licensing.CreateFilterCatalog(catalog);

            Assert.IsTrue(catalog.Parts.Any(x => x.PartType == typeof(ClassB)));
            // ClassB must not be presend because of the missing part in the licence
            Assert.IsFalse(filterCatalog.Parts.Any(x => x.PartType == typeof(ClassB)));

            CompositeApplication compositeApplication = new CompositeApplication(filterCatalog);
            licensing.AssignCompositeApplication(compositeApplication);

            ClassA classA = compositeApplication.GetExportedValue<ClassA>();

            // ClassB must be null in this case
            Assert.IsNull(classA.ClassB);
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

            private ClassB()
            {
            }

            public ClassC ClassC => classC;
        }

        [Export]
        public class ClassC
        {
            private ClassC()
            {
            }
        }
        #endregion
    }
}
