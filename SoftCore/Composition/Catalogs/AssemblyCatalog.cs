using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    public class AssemblyCatalog : Catalog
    {
        List<ComposablePart> parts = new List<ComposablePart>();

        public AssemblyCatalog(string filename, bool loadDependencyAssemblies = false)
        {
            AssemblyName an = AssemblyName.GetAssemblyName(filename);
            Assembly assembly = Assembly.Load(an);
            LoadPartsFromAssembly(assembly, loadDependencyAssemblies);
        }
        public AssemblyCatalog(Assembly assembly, bool loadDependencyAssemblies = false)
        {
            LoadPartsFromAssembly(assembly, loadDependencyAssemblies);
        }

        protected internal override IEnumerable<ComposablePart> Parts
        {
            get { return parts; }
        }

        protected internal override IEnumerable<ComposablePart> GetMatchingParts(string contractName)
        {
            var matchingParts = parts.Where(x => x.Exports.Any(e => ContractsMatch(e.ContractName, contractName)));
            return matchingParts;
        }

        private void LoadDependencyAssemblies()
        {
            throw new NotImplementedException("Loading dependency assemblies is not yet implemented.");
        }

        private void LoadPartsFromAssembly(Assembly assembly, bool loadDependencyAssemblies)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                TryLoadPart(type);
            }

            if (loadDependencyAssemblies)
            {
                var referencedAssemblies = assembly.GetReferencedAssemblies();

                foreach (var referencedAssemblyName in referencedAssemblies)
                {
                    Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);
                    LoadPartsFromAssembly(referencedAssembly, true);
                }
            }
        }

        private void TryLoadPart(Type partType)
        {
            var attributes = partType.GetTypeInfo().GetCustomAttributes<ExportAttribute>(true);

            if (attributes != null && attributes.Any())
            {
                ComposablePart part = new ComposablePart(partType);
                parts.Add(part);
            }
        }
    }
}
