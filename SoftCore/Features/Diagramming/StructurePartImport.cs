using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features.Diagramming
{
    public class StructurePartImport
    {
        public string ContractName { get; set; }
        public StructurePartImportCardinality Cardinality { get; set; }
        public bool IsOptional { get; set; }
        public string[] MatchingExports { get; set; }
    }
}
