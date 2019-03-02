using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Testing.UnitTests
{
    public class PartiallyReplacedPartTests
    {
        [Test]
        public void TestPartiallyReplacedPart()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(RealExport),
                typeof(TestClass));
            ReplacementTypeCatalog replacementCatalog = new ReplacementTypeCatalog(catalog,
                typeof(ReplacementExport));

            CompositeApplication compositeApplication = new CompositeApplication(replacementCatalog);
            TestClass testClass = compositeApplication.GetExportedValue<TestClass>();
            Assert.IsTrue(testClass.TestExport1 is ReplacementExport);
            Assert.IsTrue(testClass.TestExport2 is RealExport);
        }

        #region Classes
        interface ITestExport1
        {
        }
        interface ITestExport2
        {
        }

        [Export(typeof(ITestExport1), typeof(ITestExport2))]
        class RealExport : ITestExport1, ITestExport2
        {
            private RealExport()
            {
            }
        }
        [ReplacementExport(typeof(ITestExport1))]
        class ReplacementExport : ITestExport1
        {
            private ReplacementExport()
            {
            }
        }

        [Export]
        class TestClass
        {
            [Import(typeof(ITestExport1))]
            private ITestExport1 testExport1;
            [Import(typeof(ITestExport2))]
            private ITestExport2 testExport2;

            private TestClass()
            {
            }

            public ITestExport1 TestExport1 => testExport1;
            public ITestExport2 TestExport2 => testExport2;
        }
        #endregion
    }
}
