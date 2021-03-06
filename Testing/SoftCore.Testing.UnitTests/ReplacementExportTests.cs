using NUnit.Framework;
using SoftCore;
using SoftCore.Composition;
using SoftCore.Testing;

namespace SoftCore.Testing.UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestReplacementPart()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(RealExport),
                typeof(TestClass));
            ReplacementTypeCatalog replacementCatalog = new ReplacementTypeCatalog(catalog,
                typeof(ReplacementExport));

            CompositeApplication compositeApplication = new CompositeApplication(replacementCatalog);
            TestClass testClass = compositeApplication.GetExportedValue<TestClass>();
            Assert.IsTrue(testClass.TestExport is ReplacementExport);
        }

        #region Classes
        interface ITestExport
        {
        }

        [Export(typeof(ITestExport))]
        class RealExport : ITestExport
        {
            private RealExport()
            {
            }
        }
        [ReplacementExport(typeof(ITestExport))]
        class ReplacementExport : ITestExport
        {
            private ReplacementExport()
            {
            }
        }

        [Export]
        class TestClass
        {
            [Import(typeof(ITestExport))]
            private ITestExport testExport;

            private TestClass()
            {
            }

            public ITestExport TestExport => testExport;
        }
        #endregion
    }
}