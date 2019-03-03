using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.InterfaceBridging.UnitTests
{
    public class InterfaceBridgingTests
    {
        [Test]
        public void TestInterfaceBridging()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB));
            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            CompositeApplicationInterfaceBridging interfaceBridging = new CompositeApplicationInterfaceBridging(compositeApplication);
            interfaceBridging.EnableInterfaceBridging();

            ClassA classA = compositeApplication.GetExportedValue<ClassA>();
            string name = classA.Test.GetName();
            Assert.AreEqual(name, "Mike");
        }

        #region Classes
        // Interface bridging will only work when string is specified as a contract instead of
        // type, because types would otherwise not match.
        [Export]
        public class ClassA
        {
            [Import("MyTest")]
            private ClassA.ITest test;

            private ClassA()
            {
            }

            public ClassA.ITest Test => test;

            public interface ITest
            {
                string GetName();
            }
        }

        [Export("MyTest")]
        public class ClassB : ClassB.ITest
        {
            private ClassB()
            {
            }

            public int GetAge()
            {
                return 27;
            }

            public double GetHeight()
            {
                return 1.8;
            }

            public string GetName()
            {
                return "Mike";
            }

            public interface ITest
            {
                string GetName();
                int GetAge();
                double GetHeight();
            }
        }
    
        #endregion
    }
}
