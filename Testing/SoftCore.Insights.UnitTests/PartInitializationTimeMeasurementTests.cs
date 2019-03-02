using NUnit.Framework;
using SoftCore;
using SoftCore.Composition;
using SoftCore.Insights;
using System;
using System.Threading;

namespace SoftCore.Insights.UnitTests
{
    public class PartInitializationTimeMeasurementTests
    {
        private static int sleepTime = 200;
        private double exportAInitialisationTime = 0;
        private bool wasTimeMeasured = false;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPartInitializationTimeMeasurement()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ExportA),
                typeof(ExportB));
            CompositeApplication compositeApplication = new CompositeApplication(catalog);

            // Init insights
            CompositeApplicationsInsights insights = new CompositeApplicationsInsights(compositeApplication);
            insights.EnablePartInitializationTimeMeasurement();
            insights.PartInitializationTimeMeasured += Insights_PartInitialized;

            ExportA exportA = compositeApplication.GetExportedValue<ExportA>();
            Assert.IsTrue(wasTimeMeasured);
            // Time measured must be sleepTime +/- some error margin
            Assert.IsTrue(Math.Abs(exportAInitialisationTime - sleepTime) < 30, "Time measured was not witheen the error margin!");

            // Disable time measurement
            wasTimeMeasured = false;
            exportAInitialisationTime = 0;
            insights.DisablePartInitializationTimeMeasurement();
            ExportB exportB = compositeApplication.GetExportedValue<ExportB>();

            Assert.IsTrue(!wasTimeMeasured);
        }

        private void Insights_PartInitialized(object sender, SoftCore.Insights.Features.PartInitializedEventArgs e)
        {
            if (e.PartType == typeof(ExportA))
            {
                exportAInitialisationTime = e.TimeInMiliseconds;
                wasTimeMeasured = true;
            }
            else
                throw new Exception("Wrong part type");
        }

        #region Classes
        [Export]
        public class ExportA : IPartSatisfiedCallback
        {
            private ExportA()
            {
            }

            void IPartSatisfiedCallback.OnImportsSatisfied()
            {
                Thread.Sleep(sleepTime);
            }
        }

        [Export]
        public class ExportB
        {
            private ExportB()
            {
            }
        }
        #endregion
    }
}