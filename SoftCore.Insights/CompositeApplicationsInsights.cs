using SoftCore.Insights.Features;
using System;

namespace SoftCore.Insights
{
    public class CompositeApplicationsInsights
    {
        private CompositeApplication compositeApplication;

        private PartInitializationTimeMeasurement moduleLoadTimeMeasurement;

        public CompositeApplicationsInsights(CompositeApplication compositeApplication)
        {
            this.compositeApplication = compositeApplication;
        }

        public void EnablePartInitializationTimeMeasurement()
        {
            if (moduleLoadTimeMeasurement == null)
            {
                moduleLoadTimeMeasurement = new PartInitializationTimeMeasurement(compositeApplication);
                moduleLoadTimeMeasurement.PartInitialized += ModuleLoadTimeMeasurement_PartInitialized;
            }
        }

        public void DisablePartInitializationTimeMeasurement()
        {
            if (moduleLoadTimeMeasurement != null)
                moduleLoadTimeMeasurement.Dispose();
        }

        private void ModuleLoadTimeMeasurement_PartInitialized(object sender, PartInitializedEventArgs e)
        {
            PartInitializationTimeMeasured?.Invoke(sender, e);
        }

        public event EventHandler<PartInitializedEventArgs> PartInitializationTimeMeasured;
    }
}
