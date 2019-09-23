using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class ExportFactoryWithInheritenceTests
    {
        [Test]
        public void TestThatExportFactoryIsUsedWithConstructorParameters()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(FactoryImporterWithInheritedExport));

            CompositeApplication composer = new CompositeApplication(catalog);
            var export = composer.GetExportedValue<FactoryImporterWithInheritedExport>();

            SpecificClass sc = new SpecificClass();

            var import = export.Export.CreateExport(sc);

            Assert.IsTrue(import.SomeClass == sc);
        }

        #region Classes
        public abstract class BaseClass
        {
        }
        public class SpecificClass : BaseClass
        {
        }

        [Export]
        [NotShared]
        public class ExportA
        {
            private ExportA(BaseClass someClass)
            {
                this.SomeClass = someClass;
            }

            public BaseClass SomeClass { get; }
        }

        [Export]
        public class FactoryImporterWithInheritedExport
        {
            [Import]
            private ExportFactory<ExportA, SpecificClass> export;

            private FactoryImporterWithInheritedExport()
            {
            }

            public ExportFactory<ExportA, SpecificClass> Export => export;
        }
        #endregion
    }
}
