using NUnit.Framework;
using SoftCore.Composition;
using System.Diagnostics;

namespace SoftCore.UnitTests
{
    public class SpeedTests
    {
        [Test]
        public void SpeedTest()
        {
            TypeCatalog catalog = new TypeCatalog(
                    typeof(ClassB),
                    typeof(ClassC),
                    typeof(ClassD),
                    typeof(ClassE),
                    typeof(ClassA));

            Stopwatch clock = new Stopwatch();
            clock.Start();

            for (int i = 0; i < 1000; i++)
            {
                CompositeApplication compositeApplication = new CompositeApplication(catalog);
                ClassA importA = compositeApplication.GetExportedValue<ClassA>();
            }

            clock.Stop();
            Assert.Pass("Time: " + clock.ElapsedMilliseconds.ToString("N0") + "ms");
        }
    }

    #region Classes
    [Export]
    [NotShared]
    public class ClassA
    {
        [Import]
        private ClassB classB;

        [Import(typeof(ClassC))]
        private ClassC classC;

        [Import("MyExport")]
        private ClassD classD;

        private ClassA()
        {
        }

        public ClassB ClassB => classB;
        public ClassC ClassC => classC;
        public ClassD ClassD => classD;
    }

    [Export]
    [NotShared]
    public class ClassB
    {
        private ClassB()
        {
        }
    }
    [Export(typeof(ClassC))]
    [NotShared]
    public class ClassC
    {
        private ClassC()
        {
        }
    }
    [Export("MyExport")]
    [NotShared]
    public class ClassD
    {
        [Import]
        private ClassE classE;

        private ClassD()
        {
        }

        public ClassE ClassE => classE;
    }
    [Export]
    [NotShared]
    public class ClassE
    {
        private ClassE()
        {
        }
    }
    #endregion
}
