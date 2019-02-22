using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.UnitTests.CompositionTests
{
    public class SharedAndNotSharedTests
    {
        [Test]
        public void TestSharedParts()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(SharedExport),
                typeof(ImportShared));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);

            ImportShared import = compositeApplication.GetExportedValue<ImportShared>();
            Assert.IsNotNull(import.Export1);
            Assert.IsNotNull(import.Export2);
            Assert.IsTrue(import.Export1 == import.Export2);
        }
        [Test]
        public void TestNotSharedParts()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(NotSharedExport),
                typeof(ImportNotShared));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);

            ImportNotShared import = compositeApplication.GetExportedValue<ImportNotShared>();
            Assert.IsNotNull(import.Export1);
            Assert.IsNotNull(import.Export2);
            Assert.IsTrue(import.Export1 != import.Export2);
        }
    }

    #region Classes
    [Export]
    [NotShared]
    public class NotSharedExport
    {
        private NotSharedExport()
        {
        }
    }

    [Export]
    public class ImportNotShared
    {
        [Import]
        private NotSharedExport export1;
        [Import]
        private NotSharedExport export2;

        private ImportNotShared()
        {
        }

        public NotSharedExport Export1 => export1;
        public NotSharedExport Export2 => export2;
    }


    [Export]
    public class SharedExport
    {
        private SharedExport()
        {
        }
    }

    [Export]
    public class ImportShared
    {
        [Import]
        private SharedExport export1;
        [Import]
        private SharedExport export2;

        private ImportShared()
        {
        }

        public SharedExport Export1 => export1;
        public SharedExport Export2 => export2;
    }
    #endregion
}
