using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features.Diagramming
{
    /// <summary>
    /// Indicates the cardinality of the Export objects required by an Import
    /// </summary>
    public enum StructurePartImportCardinality
    {
        /// <summary>
        /// Zero or one Export objects are required by the Import
        /// </summary>
        ZeroOrOne = 0,
        /// <summary>
        /// Exactly one Export object is required by the ImportDefinition
        /// </summary>
        ExactlyOne = 1,
        /// <summary>
        /// Zero or more Export objects are required by the ImportDefinition.
        /// </summary>
        ZeroOrMore = 2
    }
}
