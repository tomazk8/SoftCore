using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features
{
    public class BackgroundInstantiation
    {
        private List<Type> typeList = new List<Type>();
        private CompositionContainer compositionContainer;

        public BackgroundInstantiation(CompositionContainer compositionContainer)
        {
            // Ordered list
            // All or selected classes
            // Run in multiple threads, up to MaxThread

            this.compositionContainer = compositionContainer;
        }

        public void AddType<T>()
        {
            AddType(typeof(T));
        }
        public void AddType(Type type)
        {
            typeList.Add(type);
        }

        public void InstantiateTypesAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    foreach (var part in compositionContainer.Catalog.Parts)
                    {
                        SharedLifetimeManager sharedLifetimeManager = part.LifetimeManager as SharedLifetimeManager;

                        if (sharedLifetimeManager != null)
                        {
                            sharedLifetimeManager.CreateAndCacheInstance();
                        }
                    }
                }
                catch { }
            });
        }
    }
}
