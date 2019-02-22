using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SoftCore.Composition
{
    /// <summary>
    /// Represents the part in the IoC container that has at least one export and can have imports.
    /// </summary>
    public class ComposablePart
    {
        public ComposablePart(Type partType, IEnumerable<ComposablePartExport> exports,
            IEnumerable<ComposablePartImport> imports, LifetimeManager lifetimeManager)
        {
            this.PartType = partType;
            this.Exports = exports;
            this.Imports = imports;
            this.LifetimeManager = lifetimeManager;
        }

        public ComposablePart(Type partType)
        {
            // TODO: although Part itself doesn't know anything about attributes for export, improt and lifetime,
            // the attributes are used here only to construct it. It would be wise to move this code out of
            // this class.

            this.PartType = partType;

            Exports = CompositionTools.GetExports(partType);
            Imports = CompositionTools.GetImports(partType);

            // Create lifetime manager
            NotSharedAttribute notSharedAttribute = partType.GetCustomAttribute<NotSharedAttribute>();
            LifetimeManager = notSharedAttribute != null
                ? ((LifetimeManager)new NotSharedLifetimeManager(partType))
                : ((LifetimeManager)new SharedLifetimeManager(partType));
        }
        public ComposablePart(Type partType, LifetimeManager lifetimeManager)
        {
            // TODO: although Part itself doesn't know anything about attributes for export, improt and lifetime,
            // the attributes are used here only to construct it. It would be wise to move this code out of
            // this class.

            this.PartType = partType;

            Exports = CompositionTools.GetExports(partType);
            Imports = CompositionTools.GetImports(partType);
            this.LifetimeManager = lifetimeManager;
        }

        internal LifetimeManager LifetimeManager { get; private set; }

        public Type PartType { get; private set; }
        public IEnumerable<ComposablePartExport> Exports { get; private set; }
        public IEnumerable<ComposablePartImport> Imports { get; private set; }
    }
}
