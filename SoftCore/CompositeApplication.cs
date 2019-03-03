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
        public T GetExportedValue<T>(string contractName) where T : class
        {
            return (T)container.GetExportedValue(new Contract(contractName));
        }
        public object GetExportedValue(Type type)
        {
            return container.GetExportedValue(CompositionTools.GetContractFromType(type));
        }
        public IEnumerable<T> GetExportedValues<T>() where T : class
        {
            return container.GetExportedValues<T>();
        }
        public IEnumerable<object> GetExportedValues(Type type)
        {
            return container.GetExportedValues(CompositionTools.GetContractFromType(type));
        }
        public void SatisfyImportsOnInstance(object instance)
        {
            container.SatisfyImportsOnInstance(instance);
        }

        public event EventHandler<SatisfyingImportEventArgs> SatisfyingImport
        {
            add
            {
                container.SatisfyingImport += value;
            }
            remove
            {
                container.SatisfyingImport -= value;
            }
        }
        public event EventHandler<PreRunCheckingEventArgs> PreRunChecking;
        public event EventHandler<PartCreationEventArgs> PartCreationStarted
        {
            add { container.PartCreationStarted += value; }
            remove { container.PartCreationStarted -= value; }
        }
        public event EventHandler<PartCreationEventArgs> PartCreationEnded
        {
            add { container.PartCreationEnded += value; }
            remove { container.PartCreationEnded -= value; }
        }
    }
}
