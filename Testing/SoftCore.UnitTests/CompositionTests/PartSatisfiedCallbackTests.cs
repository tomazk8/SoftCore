using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class PartSatisfiedCallbackTests
    {
        [Test]
        public void TestPartSatisfiedCallback()
        {
            TypeCatalog catalog = new TypeCatalog(
                   typeof(ExportA));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ExportA exportA = compositeApplication.GetExportedValue<ExportA>();

            Assert.AreEqual(exportA.PartSatisfied, true);
        }

        [Export]
        public class ExportA : IPartSatisfiedCallback
        {
            private ExportA()
            {
            }

            public bool PartSatisfied { get; private set; } = false;

            void IPartSatisfiedCallback.OnImportsSatisfied()
            {
                PartSatisfied = true;
            }
        }
    }
}
