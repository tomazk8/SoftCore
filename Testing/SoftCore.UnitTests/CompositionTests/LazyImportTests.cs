using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class LazyImportTests
    {
        public static bool LazyExportCreated = false;

        [Test]
        public void TestImportLazy()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(ImportA));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ImportA importA = compositeApplication.GetExportedValue<ImportA>();

            Assert.AreEqual(LazyImportTests.LazyExportCreated, false);
            ExportA exportA = importA.ExportA.Value;
            Assert.AreEqual(LazyImportTests.LazyExportCreated, true);
        }

        #region Classes
        [Export]
        public class ExportA
        {
            private ExportA()
            {
                LazyImportTests.LazyExportCreated = true;
            }
        }
        [Export]
        public class ImportA
        {
            [Import]
            private Lazy<ExportA> exportA;

            private ImportA()
            {
            }

            public Lazy<ExportA> ExportA => exportA;
        }
        #endregion
    }
}
