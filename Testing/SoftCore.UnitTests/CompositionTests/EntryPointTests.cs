using NUnit.Framework;
using NUnit.Framework.Internal;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class EntryPointTests
    {
        public static bool WasEntryPointCalled = false;

        [Test]
        public void TestEntryPoint()
        {
            TypeCatalog catalog = new TypeCatalog(
                   typeof(ExportA));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            compositeApplication.Run();

            Assert.AreEqual(WasEntryPointCalled, true);
        }

        #region Classes
        [Export(typeof(IEntryPoint))]
        public class ExportA : IEntryPoint
        {
            private ExportA()
            {
            }

            void IEntryPoint.Run()
            {
                WasEntryPointCalled = true;
            }
        }
        #endregion
    }
}
