using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Testing
{
    public class ReplacementExportAttribute : ExportAttribute
    {
        public ReplacementExportAttribute(params string[] runningContext)
            : base()
        {
            //this.RunningContext = runningContext;
        }
        public ReplacementExportAttribute(Type contractType)
            : base(contractType)
        {
        }
        public ReplacementExportAttribute(string contractName)
            : base(contractName)
        {
        }

        public bool IsReplacementExport
        {
            get { return true; }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ImportOriginalAttribute : ImportAttribute
    {
        public bool IsImportOriginal
        {
            get { return true; }
        }
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
