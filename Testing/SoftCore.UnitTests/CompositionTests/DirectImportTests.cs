using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class DirectImportTests
    {
        [Test]
        public void TestImportByTypeContract()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(ExportB),
                typeof(ExportC),
                typeof(ImportA));

            CompositeApplication composer = new CompositeApplication(catalog);
            ImportA importA = composer.GetExportedValue<ImportA>();

            Assert.IsNotNull(importA.ExportA);
            Assert.IsNotNull(importA.ExportB);
            Assert.IsNotNull(importA.ExportC);
        }

        #region Classes
        [Export]
        public class ExportA
        {
            private ExportA()
            {
            }
        }
        [Export(typeof(ExportB))]
        public class ExportB
        {
            private ExportB()
            {
            }
        }
        [Export("MyExport")]
        public class ExportC
        {
            private ExportC()
            {
            }
        }
        [Export]
        public class ImportA
        {
            [Import]
            private ExportA exportA;
            [Import(typeof(ExportB))]
            private ExportB exportB;
            [Import("MyExport")]
            private ExportC exportC;

            private ImportA()
            {
            }

            public ExportA ExportA => exportA;
            public ExportB ExportB => exportB;
            public ExportC ExportC => exportC;
        } 
        #endregion
    }
}
