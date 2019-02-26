using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SoftCore.Composition;

namespace SoftCore.Composition
{
    public class ApplicationCatalog : Catalog
    {
        List<ComposablePart> parts = new List<ComposablePart>();

        public ApplicationCatalog()
        {
            var files = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                if (!file.EndsWith(".dll") && !file.EndsWith(".exe"))
                    continue;

                try
                {
                    System.Diagnostics.Debug.WriteLine("ApplicationCatalog Scan file: " + file);
                    AssemblyName an = AssemblyName.GetAssemblyName(file);
                    Assembly assembly = Assembly.Load(an);

                    foreach (var type in assembly.ExportedTypes)
                    {
                        TryLoadPart(type);
                    }
                }
                catch
                {
                    // Ignore this assembly
                }
            }
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Collect composable parts
            /*List<Assembly> assemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var files = Directory.EnumerateFiles(path, "*.dll").ToArray();

            foreach (var file in files)
            {
                //AssemblyName an = AssemblyLoadContext.GetAssemblyName(file);
                //Assembly assembly = Assembly.Load(an);
                Assembly assembly = Assembly.

                assemblies.Add(assembly);
            }

            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.ExportedTypes)
                    {
                        TryLoadPart(type);
                    }
                }
                catch
                {
                    // Ignore this assembly
                }
            }*/
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

        public override IEnumerable<ComposablePart> Parts
        {
            get { return parts; }
        }
    }
}
