using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SoftCore.Features;

namespace SoftCore.Composition
{
    public class CompositionContainer
    {
        private Catalog catalog;
        private ComposablePart[] parts;
        private IProxyFactory proxyFactory;
        // List of exports that are floating (nobody imports them)
        public FloatingExportManager floatingExportManager;

        public CompositionContainer(Catalog catalog, IProxyFactory proxyFactory = null)
        {
            this.catalog = catalog;

            CheckCatalogParts();

            this.proxyFactory = proxyFactory;

            floatingExportManager = new FloatingExportManager(this);
        }

        public Catalog Catalog
        {
            get { return catalog; }
        }

        public T GetExportedValue<T>() where T : class
        {
            string contractName = CompositionTools.GetContractNameFromType(typeof(T));
            object instance = GetExportedValue(contractName);

            if (instance == null)
                throw new Exception("Part not found: " + contractName);

            T returnValue = instance as T;
            if (returnValue == null)
                throw new Exception(string.Format("Unable to cast {0} to {1}.", instance.GetType().Name, typeof(T).Name));

            return returnValue;
        }

        public object GetExportedValue(string contractName)
        {
            var list = GetExportsCore(contractName, ImportCardinality.ZeroOrOne, typeof(object))
                .FirstOrDefault();
            return list != null ? list.ToInstance(null) : null;
        }

        public IEnumerable<T> GetExportedValues<T>()
        {
            var values = GetExportedValues(typeof(T)).Cast<T>();
            return values;
        }

        public IEnumerable<object> GetExportedValues(Type type)
        {
            return GetExportedValues(CompositionTools.GetContractNameFromType(type));
        }
        public IEnumerable<object> GetExportedValues(string contractName)
        {
            var list = GetExportsCore(contractName, ImportCardinality.ZeroOrMore, typeof(object))
                .Select(x => x.ToInstance())
                .ToArray();

            return list;
        }

        private IEnumerable<Export> GetExportsCore(string contractName, ImportCardinality cardinality, Type importType)
        {
            lock (this)
            {
                var matchingParts = catalog.GetMatchingParts(contractName);

                switch (cardinality)
                {
                    case ImportCardinality.ExactlyOne:
                        if (matchingParts.Count() > 1)
                            throw new Exception("Too many exports found for import '" + contractName + "'. Exactly one is required.");
                        if (matchingParts.Count() == 0)
                            throw new Exception("No exports found for import '" + contractName + "'. Exactly one is required.");
                        break;
                    case ImportCardinality.ZeroOrMore:
                    case ImportCardinality.ZeroOrOne:
                        // No check needed
                        break;
                }

                IEnumerable<Export> list = matchingParts
                    .Select(part =>
                    {
                        // Export represents one specific part from which part instance can be created.
                        Export export = new Export(args =>
                            {
                                // Create instance
                                object instance = CreateInstance(part, importType, args);

                                return instance;
                            });

                        return export;
                    })
                    .ToArray();

                return list;
            }
        }

        /// <summary>
        /// Used to satisfy imports on an existing instance that is not exported.
        /// </summary>
        public void SatisfyImportsOnInstance(object instance)
        {
            // Create temporary composable part just to get a list of imports
            ComposablePart tmpPart = new ComposablePart(instance.GetType());
            SatisfyImports(instance, tmpPart);
        }

        private void SatisfyImport(ComposablePartImport import, object instance, object value)
        {
            if (!import.FieldInfo.FieldType.IsAssignableFrom(value.GetType()))
                throw new Exception(string.Format("Unable to cast {0} to {1}.", value.GetType().ToString(), import.FieldInfo.FieldType.ToString()));

            // If import imports value using an interface, trigger an event that can handle proxy generation.
            if (import.FieldInfo.FieldType.IsInterface && proxyFactory != null)
                value = proxyFactory.CreateProxy(import.FieldInfo.FieldType.GetTypeInfo(), () => instance);

            import.FieldInfo.SetValue(instance, value);
        }

        private void SatisfyImports(object instance, ComposablePart part)
        {
            foreach (var import in part.Imports)
            {
                var exports = GetExportsCore(import.ContractName, import.Cardinality, import.ImportType);

                switch (import.ImportMethod)
                {
                    case ImportMethod.Direct:
                        {
                            if (exports.Any())
                                SatisfyImport(import, instance, exports.First().ToInstance());
                            break;
                        }
                    case ImportMethod.List:
                        {
                            // Create a list with same generic argument as the type specified in imported IEnumerable field.
                            var listType = typeof(List<>);
                            var constructedType = listType.MakeGenericType(import.ImportType);
                            IList list = Activator.CreateInstance(constructedType) as IList;

                            // Fill the list
                            foreach (var export in exports)
                            {
                                list.Add(export.ToInstance());
                            }

                            SatisfyImport(import, instance, list);
                            break;
                        }
                    case ImportMethod.Lazy:
                        {
                            var lazyType = typeof(LazyExport<>);
                            var constructedLazyType = lazyType.MakeGenericType(import.ImportType);
                            Export export = exports.First();
                            object lazyInstance = Activator.CreateInstance(constructedLazyType, export);

                            SatisfyImport(import, instance, lazyInstance);

                            break;
                        }
                    case ImportMethod.ExportFactory:
                        {
                            var constructor = import.ImportType
                                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                                .SingleOrDefault();

                            int constructorArgumentCount = 0;
                            IEnumerable<Type> constructorArguments = null;

                            if (constructor != null)
                            {
                                constructorArguments = constructor
                                    .GetParameters()
                                    .Select(x => x.ParameterType);
                                constructorArgumentCount = constructorArguments.Count();
                            }

                            Type exportType;

                            // Support for parameterized constructors
                            switch (constructorArgumentCount)
                            {
                                case 0:
                                    exportType = typeof(ExportFactory<>);
                                    break;
                                case 1:
                                    exportType = typeof(ExportFactory<,>);
                                    break;
                                case 2:
                                    exportType = typeof(ExportFactory<,,>);
                                    break;
                                case 3:
                                    exportType = typeof(ExportFactory<,,,>);
                                    break;
                                case 4:
                                    exportType = typeof(ExportFactory<,,,,>);
                                    break;
                                case 5:
                                    exportType = typeof(ExportFactory<,,,,,>);
                                    break;
                                case 6:
                                    exportType = typeof(ExportFactory<,,,,,,>);
                                    break;
                                case 7:
                                    exportType = typeof(ExportFactory<,,,,,,,>);
                                    break;
                                case 8:
                                    exportType = typeof(ExportFactory<,,,,,,,,>);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                            Type[] genericTypeArguments;

                            if (constructorArguments != null)
                                genericTypeArguments = (new Type[] { import.ImportType }).Concat(constructorArguments).ToArray();
                            else
                                genericTypeArguments = new Type[1] { import.ImportType };

                            var constructedExportType = exportType.MakeGenericType(genericTypeArguments);
                            Export export = exports.First();
                            object exportFactoryInstance = Activator.CreateInstance(constructedExportType, export);

                            SatisfyImport(import, instance, exportFactoryInstance);

                            break;
                        }
                }
            }
        }

        internal object CreateInstance(ComposablePart part, Type importType, IEnumerable<object> args)
        {
            Func<object> instanceCreator = new Func<object>(() =>
            {
                InstanceInfo instanceInfo = part.LifetimeManager.GetInstance(args);

                if (instanceInfo.IsNewInstance)
                {
                    System.Diagnostics.Debug.WriteLine("CompositeContainer is initializing part " + part.PartType);

                    if (PartCreationStarted != null)
                        PartCreationStarted(this, new PartCreationEventArgs { ComposablePart = part });

                    // Satisfy imports of the instance
                    SatisfyImports(instanceInfo.Instance, part);

                    // Notify the instance that imports were satisfied
                    (instanceInfo.Instance as IPartSatisfiedCallback)?.OnImportsSatisfied();

                    if (PartCreationEnded != null)
                        PartCreationEnded(this, new PartCreationEventArgs { ComposablePart = part });
                }

                return instanceInfo.Instance;
            });

            object instance;

            if (proxyFactory != null)
                instance = proxyFactory.CreateProxy(importType.GetTypeInfo(), instanceCreator);
            else
                instance = instanceCreator();

            return instance;
        }

        /// <summary>
        /// Checks if exports for imports exist when import is not optional and checks if the number of exports match the cardinality of the import.
        /// </summary>
        private void CheckCatalogParts()
        {
            foreach (var part in catalog)
            {
                foreach (var import in part.Imports)
                {
                    IEnumerable<ComposablePart> matchingParts = catalog.GetMatchingParts(import.ContractName);

                    // Check cardinality
                    switch (import.Cardinality)
                    {
                        case ImportCardinality.ExactlyOne:
                            if (matchingParts.Count() == 0)
                                throw new Exception(string.Format("Export not found. Part: {0}, Import: {1}. Cardinality is ExactlyOne.", part.PartType, import.ContractName));
                            else if (matchingParts.Count() > 1)
                                throw new Exception(string.Format("To many exports found. Part: {0}, Import: '{1}'. Cardinality is ExactlyOne.", part.PartType, import.ContractName));
                            break;
                        case ImportCardinality.ZeroOrOne:
                            if (matchingParts.Count() > 1)
                                throw new Exception(string.Format("To many exports found. Part: {0}, Import: '{1}'. Cardinality is ExactlyOne.", part.PartType, import.ContractName));
                            break;
                    }
                }
            }
        }

        public event EventHandler<PartCreationEventArgs> PartCreationStarted;
        public event EventHandler<PartCreationEventArgs> PartCreationEnded;
    }

    public class PartCreationEventArgs
    {
        public ComposablePart ComposablePart { get; set; }
    }
}
