using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        private IEnumerable<ComposablePart> replacementParts;
        private List<ComposablePart> allParts = new List<ComposablePart>();

        public ReplacementTypeCatalog(Catalog baseCatalog, params Type[] types)
        {
            this.baseCatalog = baseCatalog;

            replacementParts = types.Select(x =>
                {
                    // Get exports marked with ReplacementExport attribute instead of a standard Export attribute
                    var exports = GetReplacementExports(x);
                    // Imports are the same
                    var imports = CompositionTools.GetImports(x);
                        return new ComposablePart(x, exports, imports, new SharedLifetimeManager(x));
                })
                .ToArray();
            allParts.AddRange(replacementParts);

            IEnumerable<Contract> replacementExports = replacementParts
                .SelectMany(x => x.Exports)
                .Select(x => x.Contract)
                .ToArray();

            foreach (var part in baseCatalog.Parts)
            {
                List<ComposablePartExport> notReplacedExports = new List<ComposablePartExport>();
                bool areExportsReplaced = false;

                foreach (var export in part.Exports)
                {
                    if (replacementExports.Contains(export.Contract))
                        areExportsReplaced = true;
                    else
                        notReplacedExports.Add(export);
                }

                if (areExportsReplaced)
                {
                    if (notReplacedExports.Any())
                    {
                        // Only some of the exports are replaced. Therefore create a new part that takes everything from
                        // replaced part, except the exports it replaces, but keeps the rest of the exports.
                        var newPart = new ComposablePart(part.PartType, notReplacedExports, part.Imports, part.LifetimeManager);
                        allParts.Add(newPart);
                    }
                }
                else
                    // None or all of the exports of this part are replaced, so add this part from the baseCatalog
                    // on the list as it is.
                    allParts.Add(part);
            }
        }

        public override IEnumerable<ComposablePart> Parts => allParts;

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
