﻿using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Testing
{
    /// <summary>
    /// The class with this attribute will be put into the IoC container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ReplacementExportAttribute : Attribute
    {
        public ReplacementExportAttribute()
        {
            Contracts = Array.Empty<Contract>();
        }
        public ReplacementExportAttribute(params string[] contractNames)
        {
            Contracts = contractNames.Select(x => new Contract(x));
        }
        public ReplacementExportAttribute(params Type[] contractTypes)
        {
            Contracts = contractTypes
                .Select(x => CompositionTools.GetContractFromType(x))
                .ToArray();
        }

        public IEnumerable<Contract> Contracts { get; internal set; }
    }

    /// <summary>
    /// Value of the field that has this attribute will be set to the instance of the composable part
    /// having export with the same contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ImportOriginalAttribute : Attribute
    {
        public ImportOriginalAttribute()
        {
            Contract = null;
        }
        public ImportOriginalAttribute(string contract)
        {
            Contract = new Contract(contract);
        }
        public ImportOriginalAttribute(Type type)
        {
            Contract = CompositionTools.GetContractFromType(type);
        }

        public Contract Contract { get; private set; }
    }

    public static class ReplacementExportFilter
    {
        public static IEnumerable<Export> FilterExportsByRunningContext(IEnumerable<Export> exports, string runningContext = null)
        {
            throw new NotImplementedException();
            /*IEnumerable<Export> retVal;

            var replacementExports = exports.Where(x =>
            {
                object value;
                bool attributeExists = x.Definition.Metadata.TryGetValue(nameof(ReplacementExportAttribute.IsReplacementExport), out value); // value will always be true for this property
                bool valueExists = x.Definition.Metadata.TryGetValue(nameof(ReplacementExportAttribute.RunningContext), out value);

                if (attributeExists && !string.IsNullOrWhiteSpace(runningContext))
                {
                    string[] runningContexts = (string[])value;

                    if (runningContexts != null)
                    {
                        return runningContexts.Any(rc => string.Equals(rc, runningContext, StringComparison.InvariantCultureIgnoreCase));
                    }
                    else
                        return false;
                }
                else
                    return false;
            });

            if (replacementExports.Any())
            {
                retVal = replacementExports;
            }
            else
            {
                var normalExports = exports.Where(x =>
                {
                    object value;
                    bool attributeExists = x.Definition.Metadata.TryGetValue("IsReplacementExport", out value); // value will always be true for this property

                    return !attributeExists;
                });

                retVal = normalExports;
            }

            return retVal;*/
        }
    }
}
