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
        private List<ComposablePart> parts = new List<ComposablePart>();
        private Dictionary<string, List<ComposablePart>> replacementParts = new Dictionary<string, List<ComposablePart>>();

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
                parts.Add(replacementPart);

                foreach (var replacementExport in replacementExports)
                {
                    List<ComposablePart> list;

                    if (!replacementParts.TryGetValue(replacementExport.ContractName, out list))
                    {
                        list = new List<ComposablePart>();
                        replacementParts.Add(replacementExport.ContractName, list);
                    }

                    list.Add(replacementPart);
                }
            }
        }

        public override IEnumerable<ComposablePart> Parts => throw new NotImplementedException();

        public override IEnumerable<ComposablePart> GetMatchingParts(string contractName)
        {
            if (replacementParts.TryGetValue(contractName, out List<ComposablePart> list))
            {
                return list;
            }
            else
            {
                var matchingParts = baseCatalog
                    .GetMatchingParts(contractName);
                return matchingParts;
            }
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
                var contractNames = attribute.ContractNames.Any() ?
                        attribute.ContractNames :
                        new string[] { CompositionTools.GetContractNameFromType(attributeOwnerType) };

                foreach (var contractName in contractNames)
                {
                    if (!list.Any(x => x.ContractName == contractName))
                        list.Add(new ComposablePartExport(contractName));
                }

                // If there are no contracts in attribute, use the type name as a contract.
                if (!list.Any())
                {
                    string contractName = CompositionTools.GetContractNameFromType(attributeOwnerType);
                    list.Add(new ComposablePartExport(contractName));
                }
            }

            return list;
        }
    }
}
