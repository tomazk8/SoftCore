using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features.Diagramming
{
    /// <summary>
    /// Stores the information about one part in the software structure
    /// </summary>
    public class StructurePart
    {
        /// <summary>
        /// Gets the ID of the part which equals to assembly qualified name.
        /// </summary>
        public string PartId
        {
            get { return AssemblyQualifiedName; }
        }
        /// <summary>
        /// Gets or sets the dependencies
        /// </summary>
        public StructurePartImport[] Imports { get; set; }
        /// <summary>
        /// Gets or sets the exported contracts
        /// </summary>
        public StructurePartExport[] Exports { get; set; }
        /// <summary>
        /// Gets or sets the class name
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// Gets or sets the assembly name
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified name
        /// </summary>
        public string AssemblyQualifiedName { get; set; }
        /// <summary>
        /// Gets or sets if access control is enabled on this part
        /// </summary>
        public bool IsAccessControlEnabled { get; set; }
    }
}
