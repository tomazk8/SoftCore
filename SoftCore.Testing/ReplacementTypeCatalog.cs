using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.Testing
{
    /// <summary>
    /// This catalog takes parts from the base catalog and replaces parts with same
    /// contract name with the parts from the replacement part catalog. The
    /// replacement parts must be tagged with <see cref="Catalog" /> ReplacementExport attribute.
    /// </summary>
    public class ReplacementTypeCatalog : Catalog
    {
        private Catalog baseCatalog;
        private List<ComposablePart> replacementParts = new List<ComposablePart>();
        private List<ComposablePart> allParts = new List<ComposablePart>();

        public ReplacementTypeCatalog(Catalog baseCatalog, params Type[] types)
        {
            this.baseCatalog = baseCatalog;

            foreach (var type in types)
            {
                // Get exports marked with ReplacementExport attribute instead of a standard Export attribute
                var replacementExports = GetReplacementExports(type);
                // Imports are the same
                var imports = CompositionTools.GetImports(type);

                ComposablePart replacementPart = new ComposablePart(type, replacementExports, imports, new SharedLifetimeManager(type));
                replacementParts.Add(replacementPart);
            }

            foreach (var part in baseCatalog)
            {
                var list = replacementParts.Where(
            }
        }

        public override IEnumerable<ComposablePart> Parts => throw new NotImplementedException();

        private IEnumerable<> (ComposablePart part1, ComposablePart part2)
        {
            return part1.Exports.Any(x => part2.Exports.Any(y => y.Contract == x.Contract));
        }

        // TODO: the method is copy pasted from SoftCore code. Try to find a way to share the same
        // functionality but with different attributes.
        private IEnumerable<ComposablePartExport> GetReplacementExports(Type partType)
        {
            //var typeInfo = partType.GetTypeInfo();
            List<ComposablePartExport> list = new List<ComposablePartExport>();

            ReplacementExportAttribute attribute = null;

            // Find the attribute and on which type it is defined
            Type attributeOwnerType = partType;
            while (attributeOwnerType != null)
            {
                attribute = attributeOwnerType.GetCustomAttribute<ReplacementExportAttribute>(false);
                if (attribute != null)
                    break;

                attributeOwnerType = attributeOwnerType.BaseType;
            }

            if (attribute != null)
            {
                // Get exported contracts from attribute
                var contracts = attribute.Contracts.Any() ?
                        attribute.Contracts :
                        new Contract[] { CompositionTools.GetContractFromType(attributeOwnerType) };

                foreach (var contract in contracts)
                {
                    if (!list.Any(x => x.Contract == contract))
                        list.Add(new ComposablePartExport(contract));
                }

                // If there are no contracts in attribute, use the type name as a contract.
                if (!list.Any())
                {
                    Contract contract = CompositionTools.GetContractFromType(attributeOwnerType);
                    list.Add(new ComposablePartExport(contract));
                }
            }

            return list;
        }
    }
}
