using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftCore.Composition;
using SoftCore.Features;

namespace SoftCore
{
    public interface ICompositeApplication
    {
        T GetExportedValue<T>() where T : class;
        object GetExportedValue(Type type);
        IEnumerable<T> GetExportedValues<T>() where T : class;
        IEnumerable<object> GetExportedValues(Type type);
        void SatisfyImportsOnInstance(object instance);
    }

    public class CompositeApplication : ICompositeApplication
    {
        private CompositionContainer container;
        private Catalog catalog;
        private ModuleLoadTimeMeasurement moduleLoadTimeMeasurement;

        public CompositeApplication(Catalog compositeCatalog)
        {
            if (this.container != null)
                throw new Exception("Application is already running");

            // Initialize catalog with internal parts
            ExplicitInstanceCatalog internalCatalog = new ExplicitInstanceCatalog(
                new ExplicitInstance(typeof(ICompositeApplication), this));
            this.catalog = new AggregateCatalog(compositeCatalog, internalCatalog);

            // Create composition container
            this.container = new CompositionContainer(this.catalog);

            PreRunChecks.PerformChecks(container);

            moduleLoadTimeMeasurement = new ModuleLoadTimeMeasurement(this.container);
        }

        public void Run()
        {
            // Run application
            IEntryPoint entryPoint = container.GetExportedValue<IEntryPoint>();
            entryPoint.Run();
        }

        public T GetExportedValue<T>() where T : class
        {
            return container.GetExportedValue<T>();
        }
        public object GetExportedValue(Type type)
        {
            return container.GetExportedValue(CompositionTools.GetContractNameFromType(type));
        }
        public IEnumerable<T> GetExportedValues<T>() where T : class
        {
            return container.GetExportedValues<T>();
        }
        public IEnumerable<object> GetExportedValues(Type type)
        {
            return container.GetExportedValues(CompositionTools.GetContractNameFromType(type));
        }
        public void SatisfyImportsOnInstance(object instance)
        {
            container.SatisfyImportsOnInstance(instance);
        }
    }
}
