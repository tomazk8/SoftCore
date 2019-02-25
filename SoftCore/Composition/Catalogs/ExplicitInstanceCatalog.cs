using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftCore.Composition;
using SoftCore.Composition.LifetimeManagement;

namespace SoftCore.Composition
{
    public class ExplicitInstanceCatalog : Catalog
    {
        private ComposablePart[] parts;

        public ExplicitInstanceCatalog(params object[] instances)
        {
            parts = instances
                .Select(x => new ComposablePart(x.GetType(), new ExplicitInstanceLifetimeManager(x)))
                .ToArray();
        }
        public ExplicitInstanceCatalog(params ExplicitInstance[] explicitInstances)
        {
            parts = explicitInstances
                .Select(x =>
                {
                    var exports = new ComposablePartExport[] { new ComposablePartExport(CompositionTools.GetContractNameFromType(x.InstanceType)) };
                    var imports = CompositionTools.GetImports(x.Instance.GetType());

                    return new ComposablePart(x.InstanceType, exports, imports, new ExplicitInstanceLifetimeManager(x.Instance));
                })
                .ToArray();
        }

        protected internal override IEnumerable<ComposablePart> Parts => parts;

        protected internal override IEnumerable<ComposablePart> GetMatchingParts(string contractName)
        {
            return parts.Where(x => x.Exports.Any(e => e.ContractName == contractName));
        }
    }
    public class ExplicitInstance
    {
        public ExplicitInstance(Type instanceType, object instance)
        {
            // Check that instance is of specified type
            if (!instanceType.IsAssignableFrom(instance.GetType()))
                throw new Exception($"Explicit instance ({instance.GetType().Name}) does not implement the specified type: {instanceType.Name}");

            this.InstanceType = instanceType;
            this.Instance = instance;
        }

        public Type InstanceType { get; private set; }
        public object Instance { get; private set; }
    }
}
