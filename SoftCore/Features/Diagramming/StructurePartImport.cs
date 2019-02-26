using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features.Diagramming
{
    public class StructurePartImport
    {
        public StructurePartImport(string contractName, StructurePartImportCardinality cardinality,
            bool isOptional, string[] matchingExports)
        {
            this.Contract = new Contract(contractName);
            this.Cardinality = cardinality;
            this.IsOptional = isOptional;
            this.MatchingExports = matchingExports;
        }

        public Contract Contract { get; private set; }
        public StructurePartImportCardinality Cardinality { get; private set; }
        public bool IsOptional { get; private set; }
        public string[] MatchingExports { get; private set; }
    }
}
