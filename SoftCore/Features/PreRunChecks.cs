using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SoftCore.Composition;

namespace SoftCore
{
    /// <summary>
    /// Checks for various things before running the application to avoid run-time failures.
    /// </summary>
    internal static class PreRunChecks
    {
        public static void PerformChecks(CompositionContainer compositionContainer)
        {
            //CheckThatControlsAreNotShared(compositionContainer);
            ExportFactoryOnlyOnNonSharedParts(compositionContainer);
            PartMustHaveOnlyOnePrivateConstructor(compositionContainer);
            CheckConstructorAndExportFactoryParameters(compositionContainer);
            CheckForCircularDependency(compositionContainer);
            // TODO: Check that importing and exporting types match.
        }

        /*/// <summary>
        /// Checks that exported controls are non-shared (they are not singletons). If they would be, that would make problems later
        /// because control can only be displayed on one parent.
        /// </summary>
        private static void CheckThatControlsAreNotShared(CompositionContainer compositionContainer)
        {
            // TODO: this is WPF specific. Move it out to other project.
            foreach (var part in compositionContainer.Catalog)
            {
                var partType = part.PartName;
                string controlTypeName = null;
                bool isNonShared = false;

                if (partType != null)
                {
                    for (Type t = partType; t != null; t = t.GetTypeInfo().BaseType)
                    {
                        if (t.FullName == "System.Windows.Media.Visual")
                        {
                            controlTypeName = partType.FullName;
                            // TODO: get creation policy for this part
                            isNonShared = true; // part.CreationPolicy == CreationPolicy.NonShared;

                            break;
                        }
                    }
                }

                if (controlTypeName != null && !isNonShared)
                    throw new Exception("Exported control is not marked as NonShared: " + controlTypeName);
            }
        }*/

        /// <summary>
        /// Checks if all required referenced assemblies and their versions are present.
        /// </summary>
        private static void CheckReferencedAssemblies(CompositionContainer compositionContainer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If ExportFactory is used to import a part, this part must not be shared (singleton). The reason is that
        /// if it is designed as singleton, it doesn't go together with a factory pattern because factory will
        /// always create a new instance when invoked.
        /// </summary>
        private static void ExportFactoryOnlyOnNonSharedParts(CompositionContainer compositionContainer)
        {
            foreach (var part in compositionContainer.Catalog)
            {
                foreach (var partImport in part.Imports)
                {
                    if (partImport.ImportMethod == ImportMethod.ExportFactory)
                    {
                        var exportedParts = compositionContainer.Catalog
                            .Where(x => x.Exports.Any(e => partImport.MatchesWith(e)));

                        if (exportedParts.Any(x => x.LifetimeManager.GetType() == typeof(SharedLifetimeManager)))
                            throw new Exception($"ExportFactory cannot import a part that is shared. Use {nameof(NotSharedAttribute)} on this class.");
                    }
                }
            }
        }

        /// <summary>
        /// Since AppComposes uses private-field injection, only one constructor is required. Constructor must also
        /// be private because this forces the developer to get an instance only by using the library. This applies
        /// also for unit tests.
        /// </summary>
        private static void PartMustHaveOnlyOnePrivateConstructor(CompositionContainer compositionContainer)
        {
            foreach (var part in compositionContainer.Catalog)
            {
                if (part.PartType == typeof(CompositeApplication))
                    continue;

                // Private constructor is required only if part uses Shared or NotShared lifetime
                // managers. If any other lifetime manager is used, having a private constructor
                // doesn't make much sense.
                // TODO: does any of these makes sense?
                if (part.LifetimeManager is NotSharedLifetimeManager ||
                    part.LifetimeManager is SharedLifetimeManager)
                {
                    var count = part.PartType
                        .GetConstructors()
                        .Count();

                    if (count > 1)
                        throw new Exception($"Part {part.PartType.Name} has more than one constructor. Parts can have only one constructor that is private.");

                    var constructor = part.PartType
                        .GetConstructors()
                        .SingleOrDefault();

                    if (constructor != null && !constructor.IsPrivate)
                        throw new Exception($"Constructor on part {part.PartType.Name} must be private.");
                }
            }
        }

        /// <summary>
        /// When a constructor has parameters, check that ExportFactory is used and that constructor parameters match
        /// the ExportFactory parameters.
        /// </summary>
        private static void CheckConstructorAndExportFactoryParameters(CompositionContainer compositionContainer)
        {
            foreach (var part in compositionContainer.Catalog)
            {
                foreach (var partImport in part.Imports)
                {
                    var exportedParts = compositionContainer.Catalog
                        .Where(x => x.Exports.Any(e => partImport.MatchesWith(e)));

                    foreach (var exportedPart in exportedParts)
                    {
                        var constructorInfo = exportedPart.PartType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();
                        var constructorParams = constructorInfo
                            .GetParameters()
                            .ToArray();

                        if (!constructorParams.Any())
                            continue;

                        // When constructor has parameters, ExportFactory must be used because this is the only
                        // way to specify these parameters.
                        if (partImport.ImportMethod != ImportMethod.ExportFactory)
                            throw new Exception($"Class {part.PartType.Name} imports class {exportedPart.PartType.Name} which has constructor parameters. The import must therefore use ExportFactory.");

                        // Check that parameters match
                        var exportArguments = partImport.FieldInfo.FieldType
                            .GetGenericArguments()
                            .Skip(1) // Skip the first argument because it is not a constructor parameter
                            .ToArray();

                        bool parametersValid = true;

                        if (constructorParams.Length == exportArguments.Length)
                        {
                            for (int i = 0; i < constructorParams.Length; i++)
                            {
                                if (!constructorParams[i].ParameterType.IsAssignableFrom(exportArguments[i]))
                                    parametersValid = false;
                            }
                        }
                        else
                            parametersValid = false;

                        if (!parametersValid)
                            throw new Exception($"Class {part.PartType.Name} imports class {exportedPart.PartType.Name} that has constructor parameters but they don't match the ExportFactory parameters.");
                    }
                }
            }
        }

        /// <summary>
        /// Checks if classes are wired in a circular depencendy.
        /// </summary>
        private static void CheckForCircularDependency(CompositionContainer compositionContainer)
        {
            HashSet<string> scannedTypes = new HashSet<string>();
            Queue<ComposablePart> queue = new Queue<ComposablePart>(compositionContainer.Catalog);

            while (queue.Any())
            {
                var part = queue.Dequeue();
                ScanPart(part, compositionContainer.Catalog, Array.Empty<Type>());

                /*foreach (var partImport in part.Imports)
                {
                    var exportedParts = compositionContainer.Catalog
                        .Where(x => x.Exports.Any(e => partImport.MatchesWith(e)));

                    foreach (var exportedPart in exportedParts)
                    {
                        if (scannedTypes.Contains(exportedPart.PartType.FullName))
                            throw new Exception("");
                    }
                }*/
            }
        }

        private static void ScanPart(ComposablePart part, Catalog catalog, IEnumerable<Type> typeChain)
        {
            typeChain = typeChain.Concat(new Type[] { part.PartType });

            foreach (var partImport in part.Imports)
            {
                var exportedParts = catalog
                    .Where(x => x.Exports.Any(e => partImport.MatchesWith(e)));

                foreach (var exportedPart in exportedParts)
                {
                    if (typeChain.Contains(exportedPart.PartType))
                    {
                        string path = typeChain
                            .Concat(new Type[] { exportedPart.PartType })
                            .Select(x => x.Name)
                            .Aggregate((a, b) => a + " -> " + b);

                        throw new Exception("Circular dependency detected: " + path);
                    }

                    ScanPart(exportedPart, catalog, typeChain);
                }
            }
        }
    }
}
