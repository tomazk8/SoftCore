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
}
