using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class CircularDepencencyTests
    {
        [Test]
        public void TestCircularDependency()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB),
                typeof(ClassC));

            Assert.Catch(() =>
            {
                CompositeApplication compositeApplication = new CompositeApplication(catalog);
            });
        }
    }

    [Export]
    public class ClassA
    {
        [Import]
        private ClassB ImportedClass;

        private ClassA()
        {
        }
    }
    [Export]
    public class ClassB
    {
        [Import]
        private ClassC ImportedClass;

        private ClassB()
        {
        }
    }
    [Export]
    public class ClassC
    {
        [Import]
        private ClassA ImportedClass;

        private ClassC()
        {
        }
    }
}
