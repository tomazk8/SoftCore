using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features.Diagramming
{
    public class StructurePartExport
    {
        public Guid PartId { get; set; }
        public string ContractName { get; set; }
        public StructurePartExportCreationPolicy CreationPolicy { get; set; }
    }
}
