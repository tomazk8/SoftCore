using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SoftCore.Composition;

namespace SoftCore.Features.Diagramming
{
    /*/// <summary>
    /// Stores the information about the software structure
    /// </summary>
    public class StructureGraph
    {
        /// <summary>
        /// Gets or sets all of the parts in the software structure
        /// </summary>
        public List<StructurePart> Parts { get; set; }
        public int GraphVersion { get; set; }

        public static StructureGraph Create(Catalog catalog)
        {
            StructureGraph graph = new StructureGraph()
            {
                Parts = new List<StructurePart>(),
                GraphVersion = 1
            };

            // Create a list of parts
            foreach (var part in catalog.Parts)
            {
                TypeInfo partType = part.PartType.GetTypeInfo();
                    var attributes = partType.GetCustomAttributes(true);

                    bool isAccessControlEnabled = false;// attributes.Any(x => x is SecureAccessAttribute);

                    StructurePart structurePart = new StructurePart
                    {
                        AssemblyName = partType.Assembly.FullName,
                        AssemblyQualifiedName = partType.AssemblyQualifiedName,
                        ClassName = partType.Name,
                        IsAccessControlEnabled = isAccessControlEnabled
                    };

                    // List imports and find exports for imports
                    List<StructurePartImport> importList = new List<StructurePartImport>();
                    foreach (var partImport in part.Imports)
                    {
                        StructurePartImport import = new StructurePartImport
                        {
                            ContractName = partImport.Contract,
                            Cardinality = (StructurePartImportCardinality)partImport.Cardinality,
                            IsOptional = partImport.Cardinality == ImportCardinality.ZeroOrMore || partImport.Cardinality == ImportCardinality.ZeroOrOne,
                            Metadata = new Dictionary<string, object>(partImport.Metadata)
                        };

                        // Find parts with an export that matches current import
                        List<string> matchingExports = new List<string>();
                        foreach (var tmpPart in catalog.Parts)
                        {
                            if (tmpPart.Exports.Any(x => partImport.MatchesWith(x)))
                            {
                                // Assembly qualified name represents the ID of the part
                                matchingExports.Add(part.PartType.AssemblyQualifiedName);
                            }
                        }

                        import.MatchingExports = matchingExports.ToArray();

                        importList.Add(import);
                    }

                    structurePart.Imports = importList.ToArray();

                    graph.Parts.Add(structurePart);
            }

            return graph;
        }
        public static StructureGraph Create(string filename)
        {
            StructureGraph graph;

            using (var stream = File.Create(filename))
            {
                JsonSerializer serializer = new JsonSerializer();

                using (TextReader reader = new StreamReader(stream))
                {
                    using (JsonReader jsonReader = new JsonTextReader(reader))
                    {
                        graph = serializer.Deserialize<StructureGraph>(jsonReader);
                    }
                }
            }

            return graph;
        }

        public void Save(string filename)
        {
            using (var stream = File.Create(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;

                using (TextWriter writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, this);
                }

                stream.Flush();
            }
        }
    }*/
}
