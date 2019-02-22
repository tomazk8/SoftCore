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
