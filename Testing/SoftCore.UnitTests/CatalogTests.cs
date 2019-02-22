using NUnit.Framework;
using SoftCore;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Tests.CatalogTesting
{
    public class CatalogTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestTypeCatalog()
        {
            Catalog catalog = new TypeCatalog(typeof(ClassA), typeof(ClassB));

            Assert.IsTrue(catalog.Count() == 2);
            Assert.IsTrue(catalog.Any(x => x.PartType == typeof(ClassA)));
            Assert.IsTrue(catalog.Any(x => x.PartType == typeof(ClassB)));
        }

        /*[Test]
        public void TestAssemblyCatalog()
        {
            // TODO
        }

        [Test]
        public void TestApplicationCatalog()
        {
            // TODO
        }*/

        [Test]
        public void TestAggregateCatalog()
        {
            Catalog catalog1 = new TypeCatalog(typeof(ClassA));
            Catalog catalog2 = new TypeCatalog(typeof(ClassB));
            AggregateCatalog catalog = new AggregateCatalog(catalog1, catalog2);

            Assert.IsTrue(catalog.Count() == 2);
            Assert.IsTrue(catalog.Any(x => x.PartType == typeof(ClassA)));
            Assert.IsTrue(catalog.Any(x => x.PartType == typeof(ClassB)));
        }

        [Test]
        public void TestExplicitInstanceCatalog()
        {
            ClassA instanceA = new ClassA();
            ClassB instanceB = new ClassB();

            Catalog catalog = new ExplicitInstanceCatalog(
                new ExplicitInstance(typeof(ClassA), instanceA),
                new ExplicitInstance(typeof(ClassB), instanceB));

            CompositeApplication composer = new CompositeApplication(catalog);
            var otherInstanceA = composer.GetExportedValue<ClassA>();
            var otherInstanceB = composer.GetExportedValue<ClassB>();

            Assert.IsTrue(instanceA == otherInstanceA);
            Assert.IsTrue(instanceB == otherInstanceB);
        }

        [Test]
        public void TestFiltercatalog()
        {
            Catalog baseCatalog = new TypeCatalog(typeof(ClassA), typeof(ClassB));
            // Filter out ClassB from the catalog
            Catalog catalog = new FilterCatalog(baseCatalog, part => part.PartType != typeof(ClassB));

            Assert.IsTrue(catalog.Count() == 1);
            Assert.IsTrue(catalog.Any(x => x.PartType == typeof(ClassA)));
        }

        /*[Test]
        public void TestReplacementCatalog()
        {
            // TODO
        }*/
    }

    [Export]
    public class ClassA
    {
    }
    [Export]
    public class ClassB
    {
    }
}
