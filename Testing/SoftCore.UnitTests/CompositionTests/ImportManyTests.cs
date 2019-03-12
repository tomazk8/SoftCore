using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class ImportManyTests
    {
        [Test]
        public void TestImportMany()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ImportA),
                typeof(ExportA),
                typeof(ExportB));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ImportA importA = compositeApplication.GetExportedValue<ImportA>();

            Assert.IsTrue(importA.List.Count() == 2);
        }
        [Test]
        public void TestImportManyLazy()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ImportLazyA),
                typeof(ExportA),
                typeof(ExportB));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ImportLazyA importA = compositeApplication.GetExportedValue<ImportLazyA>();

            Assert.IsTrue(importA.List.Count() == 2);

            ExportBase export1 = importA.List.First().Value;
            ExportBase export2 = importA.List.Last().Value;
            Assert.IsTrue((export1 is ExportA && export2 is ExportB) || (export1 is ExportB && export2 is ExportA));
        }
        [Test]
        public void TestImportManyFactory()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ImportFactoryA),
                typeof(ExportA),
                typeof(ExportB));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ImportFactoryA importA = compositeApplication.GetExportedValue<ImportFactoryA>();

            Assert.IsTrue(importA.List.Count() == 2);

            ExportBase export1 = importA.List.First().CreateExport();
            ExportBase export2 = importA.List.Last().CreateExport();
            Assert.IsTrue((export1 is ExportA && export2 is ExportB) || (export1 is ExportB && export2 is ExportA));
        }
        [Test]
        public void TestImportManyWithTypeFilterFactory()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ImportFactoryA),
                typeof(ExportA),
                typeof(ExportB));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ImportFactoryA importA = compositeApplication.GetExportedValue<ImportFactoryA>();

            Assert.IsTrue(importA.List.Count() == 2);

            {
                var exportFactoryA = importA.List.Single(x => x.ExportType == typeof(ExportA));
                ExportBase exportA = exportFactoryA.CreateExport();
                Assert.IsNotNull(exportA);
                Assert.AreEqual(exportA.GetType(), typeof(ExportA));
            }

            {
                var exportFactoryB = importA.List.Single(x => x.ExportType == typeof(ExportB));
                ExportBase exportB = exportFactoryB.CreateExport();
                Assert.IsNotNull(exportB);
                Assert.AreEqual(exportB.GetType(), typeof(ExportB));
            }
        }

        #region Classes
        [Export]
        public class ImportA
        {
            [ImportMany]
            private IEnumerable<ExportBase> list;

            private ImportA()
            {
            }

            public IEnumerable<ExportBase> List => list;
        }
        [Export]
        public class ImportLazyA
        {
            [ImportMany]
            private IEnumerable<Lazy<ExportBase>> list;

            private ImportLazyA()
            {
            }

            public IEnumerable<Lazy<ExportBase>> List => list;
        }
        [Export]
        public class ImportFactoryA
        {
            [ImportMany]
            private IEnumerable<ExportFactory<ExportBase>> list;

            private ImportFactoryA()
            {
            }

            public IEnumerable<ExportFactory<ExportBase>> List => list;
        }

        [Export]
        [NotShared]
        public abstract class ExportBase
        {
            internal ExportBase()
            {
            }
        }
        public class ExportA : ExportBase
        {
            private ExportA()
            {
            }
        }
        public class ExportB : ExportBase
        {
            private ExportB()
            {
            }
        }
        #endregion
    }
}
