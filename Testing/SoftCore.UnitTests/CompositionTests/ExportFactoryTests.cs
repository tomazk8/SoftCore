using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class ExportFactoryTests
    {
        [Test]
        public void TestExportFactory()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(FactoryImporter));

            CompositeApplication composer = new CompositeApplication(catalog);
            FactoryImporter factoryImporter = composer.GetExportedValue<FactoryImporter>();

            ExportA exportA = factoryImporter.ExportAFactory.CreateExport();

            Assert.IsNotNull(exportA);
        }

        [Test]
        public void TestExportFactoryWithConstructorParameters()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(ExportB),
                typeof(ExportC),
                typeof(FactoryImporter));

            CompositeApplication composer = new CompositeApplication(catalog);
            FactoryImporter factoryImporter = composer.GetExportedValue<FactoryImporter>();

            ExportA exportA = factoryImporter.ExportAFactory.CreateExport();
            ExportB exportB = factoryImporter.ExportBFactory.CreateExport("Mike");
            ExportC exportC = factoryImporter.ExportCFactory.CreateExport("Veronique", 27);

            Assert.IsNotNull(exportA);
            Assert.IsNotNull(exportB);
            Assert.IsNotNull(exportC);
            Assert.AreEqual(exportB.Name, "Mike");
            Assert.AreEqual(exportC.Name, "Veronique");
            Assert.AreEqual(exportC.Age, 27);
        }

        #region Classes
        [Export]
        [NotShared]
        public class ExportA
        {
            private ExportA()
            {
            }
        }
        [Export(typeof(ExportB))]
        [NotShared]
        public class ExportB
        {
            private ExportB(string name)
            {
                this.Name = name;
            }

            public string Name { get; private set; }
        }
        [Export("MyExport")]
        [NotShared]
        public class ExportC
        {
            private ExportC(string name, int age)
            {
                this.Name = name;
                this.Age = age;
            }

            public string Name { get; private set; }
            public int Age { get; private set; }
        }
        [Export]
        public class FactoryImporter
        {
            [Import]
            private ExportFactory<ExportA> exportAFactory;
            [Import(typeof(ExportB))]
            private ExportFactory<ExportB, string> exportBFactory;
            [Import("MyExport")]
            private ExportFactory<ExportC, string, int> exportCFactory;

            private FactoryImporter()
            {
            }

            public ExportFactory<ExportA> ExportAFactory => exportAFactory;
            public ExportFactory<ExportB, string> ExportBFactory => exportBFactory;
            public ExportFactory<ExportC, string, int> ExportCFactory => exportCFactory;
        }
        #endregion
    }
}
