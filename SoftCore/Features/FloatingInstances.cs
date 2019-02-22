using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SoftCore.Composition;

namespace SoftCore.Features
{
    public class FloatingExportManager
    {
        private CompositionContainer compositionContainer;
        private IEnumerable<object> floatingExportInstances;

        public FloatingExportManager(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;

            CreateFloatingInstances();
        }

        private void CreateFloatingInstances()
        {
            // Gather the exports that are marked as floating, create their instances and satisfy their improts.
            // The list is then stored in memory.

            List<object> list = new List<object>();

            foreach (var part in compositionContainer.Catalog)
            {
                var floatingInstanceAttribute = part.PartType.GetCustomAttribute<FloatingExportAttribute>();

                if (floatingInstanceAttribute != null)
                {
                    var instance = compositionContainer.CreateInstance(part, part.PartType, null);
                    list.Add(instance);
                }
            }

            floatingExportInstances = list;
        }
    }
}
