using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class OptionalImportTests
    {
        [Test]
        public void TestOptionalImport()
        {
            TypeCatalog catalog = new TypeCatalog(
                   typeof(ImportA));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ImportA importA = compositeApplication.GetExportedValue<ImportA>();

            Assert.IsNull(importA.ExportA);
        }

        #region Classes
        [Export]
        public class ExportA
        {
            private ExportA()
            {
            }
        }
        [Export]
        public class ImportA
        {
            [Import(isOptional: true)]
            private ExportA exportA;

            private ImportA()
            {
            }

            public ExportA ExportA => exportA;
        }
        #endregion
    }
}
