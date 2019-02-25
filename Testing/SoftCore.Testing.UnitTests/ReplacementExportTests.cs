using NUnit.Framework;
using SoftCore;
using SoftCore.Composition;
using SoftCore.Testing;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestReplacementExport()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(RealExport),
                typeof(TestClass));
            ReplacementTypeCatalog replacementCatalog = new ReplacementTypeCatalog(catalog,
                typeof(ReplacementExport));

            CompositeApplication compositeApplication = new CompositeApplication(replacementCatalog);
            TestClass testClass = compositeApplication.GetExportedValue<TestClass>();
            Assert.IsTrue(testClass.TestExport is RealExport);
        }

        #region Classes
        interface ITestExport
        {
            bool IsReplacement { get; }
        }

        [Export(typeof(ITestExport))]
        class RealExport : ITestExport
        {
            private RealExport()
            {
            }

            public bool IsReplacement => false;
        }
        [Export(typeof(ITestExport))]
        class ReplacementExport : ITestExport
        {
            private ReplacementExport()
            {
            }

            public bool IsReplacement => true;
        }

        [Export]
        class TestClass
        {
            [Import(typeof(ITestExport))]
            private ITestExport testExport;

            public ITestExport TestExport => testExport;
        }
        #endregion
    }
}