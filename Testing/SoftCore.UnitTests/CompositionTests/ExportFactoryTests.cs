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

        [Test]
        public void TestIfNumberOfConstructorParameterCountMatch()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportC),
                typeof(FactoryImporterWithInvalidParameterCount));

            Assert.Catch(new TestDelegate(() =>
            {
                CompositeApplication composer = new CompositeApplication(catalog);
            }));
        }
        [Test]
        public void TestIfConstructorParameterTypeMatches()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportC),
                typeof(FactoryImporterWithInvalidParameterType));

            Assert.Catch(new TestDelegate(() =>
            {
                CompositeApplication composer = new CompositeApplication(catalog);
            }));
        }
        [Test]
        public void TestThatExportFactoryIsUsedWithConstructorParameters()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportC),
                typeof(FactoryImporterWithMissingExportFactory));

            Assert.Catch(new TestDelegate(() =>
            {
                CompositeApplication composer = new CompositeApplication(catalog);
            }));
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
        [Export]
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
            [Import]
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
        [Export]
        public class FactoryImporterWithInvalidParameterCount
        {
            [Import("MyExport")]
            private ExportFactory<ExportC, string> exportCFactory;

            private FactoryImporterWithInvalidParameterCount()
            {
            }

            public ExportFactory<ExportC, string> ExportCFactory => exportCFactory;
        }
        [Export]
        public class FactoryImporterWithInvalidParameterType
        {
            [Import("MyExport")]
            private ExportFactory<ExportC, string, string> exportCFactory;

            private FactoryImporterWithInvalidParameterType()
            {
            }

            public ExportFactory<ExportC, string, string> ExportCFactory => exportCFactory;
        }
        [Export]
        public class FactoryImporterWithMissingExportFactory
        {
            [Import("MyExport")]
            private ExportC exportC;

            private FactoryImporterWithMissingExportFactory()
            {
            }

            public ExportC ExportC => exportC;
        }
        #endregion
    }
}
