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
        // List of exports that are floating (nobody imports them)
        public FloatingExportManager floatingExportManager;

        public CompositionContainer(Catalog catalog)
        {
            this.catalog = catalog;

            CheckCatalogParts();

            PreRunChecking?.Invoke(this, new PreRunCheckingEventArgs(catalog));

            floatingExportManager = new FloatingExportManager(this);
        }

        public Catalog Catalog
        {
            get { return catalog; }
        }

        public T GetExportedValue<T>() where T : class
        {
            Contract contract = CompositionTools.GetContractFromType(typeof(T));
            object instance = GetExportedValue(contract);

            if (instance == null)
                throw new Exception("Part not found: " + contract.ToString());

            T returnValue = instance as T;
            if (returnValue == null)
                throw new Exception(string.Format("Unable to cast {0} to {1}.", instance.GetType().Name, typeof(T).Name));

            return returnValue;
        }

        public object GetExportedValue(Contract contract)
        {
            var list = GetExportsCore(contract, ImportCardinality.ZeroOrOne, typeof(object))
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
            return GetExportedValues(CompositionTools.GetContractFromType(type));
        }
        public IEnumerable<object> GetExportedValues(Contract contract)
        {
            var list = GetExportsCore(contract, ImportCardinality.ZeroOrMore, typeof(object))
                .Select(x => x.ToInstance())
                .ToArray();

            return list;
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

        private IEnumerable<Export> GetExportsCore(Contract contract, ImportCardinality cardinality, Type importType)
        {
            lock (this)
            {
                var matchingParts = GetMatchingParts(catalog, contract);

                switch (cardinality)
                {
                    case ImportCardinality.ExactlyOne:
                        if (matchingParts.Count() > 1)
                            throw new Exception("Too many exports found for import '" + contract.ToString() + "'. Exactly one is required.");
                        if (matchingParts.Count() == 0)
                            throw new Exception("No exports found for import '" + contract.ToString() + "'. Exactly one is required.");
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
                            },
                            part.PartType);

                        return export;
                    })
                    .ToArray();

                return list;
            }
        }

        private IEnumerable<ComposablePart> GetMatchingParts(Catalog catalog, Contract contract)
        {
            foreach (var part in catalog)
            {
                if (part.Exports.Any(x => x.Contract == contract))
                    yield return part;
            }
        }

        private void SatisfyImport(ComposablePartImport import, object instance, IEnumerable<object> fieldValues)
        {
            if (!fieldValues.Any())
                return;

            object fieldValue = null;

            if (import.Cardinality == ImportCardinality.ZeroOrMore)
            {
                // Create a list with same generic argument as the type specified in imported IEnumerable field.
                var listType = typeof(List<>);
                Type genericType;

                switch (import.ImportMethod)
                {
                    case ImportMethod.Direct:
                        {
                            genericType = import.ImportType;
                            break;
                        }
                    case ImportMethod.ExportFactory:
                        {
                            var innerType = typeof(ExportFactory<>);
                            genericType = innerType.MakeGenericType(import.ImportType);
                            break;
                        }
                    case ImportMethod.Lazy:
                        {
                            var innerType = typeof(Lazy<>);
                            genericType = innerType.MakeGenericType(import.ImportType);
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }

                var constructedType = listType.MakeGenericType(genericType);
                IList list = Activator.CreateInstance(constructedType) as IList;

                // Fill the list
                foreach (var tmpValue in fieldValues)
                {
                    list.Add(tmpValue);
                }

                fieldValue = list;
            }
            else
            {
                fieldValue = fieldValues.FirstOrDefault();
            }

            fieldValue = InvokeSatisfyingImportEvent(import.FieldInfo, fieldValue);

            if (!import.FieldInfo.FieldType.IsAssignableFrom(fieldValue.GetType()))
                throw new Exception(string.Format("Unable to cast {0} to {1}.", fieldValue.GetType().ToString(), import.FieldInfo.FieldType.ToString()));

            import.FieldInfo.SetValue(instance, fieldValue);
        }

        private void SatisfyImports(object instance, ComposablePart part)
        {
            foreach (var import in part.Imports)
            {
                var exports = GetExportsCore(import.Contract, import.Cardinality, import.ImportType);

                switch (import.ImportMethod)
                {
                    case ImportMethod.Direct:
                        {
                            if (exports.Any())
                                SatisfyImport(import, instance, exports.Select(x => x.ToInstance()));
                            break;
                        }
                    case ImportMethod.Lazy:
                        {
                            var lazyType = typeof(LazyExport<>);
                            var constructedLazyType = lazyType.MakeGenericType(import.ImportType);
                            
                            var lazyInstances = exports
                                .Select(x => Activator.CreateInstance(constructedLazyType, x));

                            SatisfyImport(import, instance, lazyInstances);

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
                            
                            var instances = exports
                                .Select(x => Activator.CreateInstance(constructedExportType, x));

                            SatisfyImport(import, instance, instances);

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

            return instanceCreator();
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
                    IEnumerable<ComposablePart> matchingParts = GetMatchingParts(catalog, import.Contract);

                    // Check cardinality
                    switch (import.Cardinality)
                    {
                        case ImportCardinality.ExactlyOne:
                            if (matchingParts.Count() == 0)
                                throw new Exception(string.Format("Export not found. Part: {0}, Import: {1}. Cardinality is ExactlyOne.", part.PartType, import.Contract));
                            else if (matchingParts.Count() > 1)
                                throw new Exception(string.Format("To many exports found. Part: {0}, Import: '{1}'. Cardinality is ExactlyOne.", part.PartType, import.Contract));
                            break;
                        case ImportCardinality.ZeroOrOne:
                            if (matchingParts.Count() > 1)
                                throw new Exception(string.Format("To many exports found. Part: {0}, Import: '{1}'. Cardinality is ExactlyOne.", part.PartType, import.Contract));
                            break;
                    }
                }
            }
        }

        public event EventHandler<PartCreationEventArgs> PartCreationStarted;
        public event EventHandler<PartCreationEventArgs> PartCreationEnded;
        public event EventHandler<PreRunCheckingEventArgs> PreRunChecking;

        private object InvokeSatisfyingImportEvent(FieldInfo fieldInfo, object instance)
        {
            object newInstance = instance;

            foreach (var item in satisfyingImportInvocationList)
            {
                var args = new SatisfyingImportEventArgs(fieldInfo) { Instance = instance };
                item(this, args);
                newInstance = args.Instance;
            }

            return newInstance;
        }

        private LinkedList<EventHandler<SatisfyingImportEventArgs>> satisfyingImportInvocationList =
            new LinkedList<EventHandler<SatisfyingImportEventArgs>>();

        public event EventHandler<SatisfyingImportEventArgs> SatisfyingImport
        {
            add
            {
                if (!satisfyingImportInvocationList.Contains(value))
                    satisfyingImportInvocationList.AddLast(value);
            }
            remove
            {
                satisfyingImportInvocationList.Remove(value);
            }
        }
    }

    public class PartCreationEventArgs : EventArgs
    {
        public ComposablePart ComposablePart { get; set; }
    }

    public class SatisfyingImportEventArgs : EventArgs
    {
        public SatisfyingImportEventArgs(FieldInfo fieldInfo)
        {
            this.FieldInfo = fieldInfo;
        }

        public FieldInfo FieldInfo { get; private set; }
        public object Instance { get; set; }
    }

    public class PreRunCheckingEventArgs : EventArgs
    {
        public PreRunCheckingEventArgs(Catalog catalog)
        {
            this.Catalog = catalog;
        }

        public Catalog Catalog { get; private set; }
    }
}
