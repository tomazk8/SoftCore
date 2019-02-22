using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class FloatingExportTests
    {
        [Test]
        public void TestFloatingExport()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(ExportB));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            ExportA exportA = compositeApplication.GetExportedValue<ExportA>();

            Assert.AreEqual(exportA.Value, 1234);
        }

        #region Classes
        [Export]
        [FloatingExport]
        public class ExportB : IPartSatisfiedCallback
        {
            [Import]
            private ExportA importA;

            private ExportB()
            {
            }

            void IPartSatisfiedCallback.OnImportsSatisfied()
            {
                importA.SetValue(1234);
            }
        }

        [Export]
        public class ExportA
        {
            public void SetValue(int value)
            {
                this.Value = value;
            }

            private ExportA()
            {
            }

            public int Value { get; private set; }
        }
        #endregion
    }
}
