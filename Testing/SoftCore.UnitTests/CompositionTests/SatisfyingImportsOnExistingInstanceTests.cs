using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class SatisfyingImportsOnExistingInstanceTests
    {
        [Test]
        public void TestSatisfyingImportsOnExistingInstance()
        {
            ExistingClass instance = new ExistingClass();

            TypeCatalog catalog = new TypeCatalog(
                typeof(ImportA));
            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            compositeApplication.SatisfyImportsOnInstance(instance);

            Assert.IsNotNull(instance.ImportA);
        }

        #region Classes
        public class ExistingClass
        {
            [Import]
            private ImportA importA;

            public ImportA ImportA => importA;
        }

        [Export]
        public class ImportA
        {
            private ImportA()
            {
            }
        }
        #endregion
    }
}
