using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    /// <summary>
    /// This lifetime manager will create only one instance of each type per application. Instance
    /// will live as long as application is running.
    /// </summary>
    public class SharedLifetimeManager : LifetimeManager
    {
        private Dictionary<Type, object> instances = new Dictionary<Type, object>();
        private Type partType;

        public SharedLifetimeManager(Type partType)
        {
            this.partType = partType;
        }

        public override InstanceInfo GetInstance(IEnumerable<object> args)
        {
            if (args != null && args.Any())
                throw new Exception($"Shared part '{partType.Name}' must have a parameterless constructor");

            lock (this)
            {
                object instance = null;

                if (instances.ContainsKey(partType))
                    instance = instances[partType];

                bool isNewInstance = false;

                if (instance == null)
                {
                    var constructor = partType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault();

                    if (constructor != null)
                        instance = constructor.Invoke(Array.Empty<object>());
                    else
                        instance = Activator.CreateInstance(partType);

                    instances.Add(partType, instance);
                    isNewInstance = true;
                }

                return new InstanceInfo(instance, isNewInstance);
            }
        }

        /// <summary>
        /// Creates an instance of this class imidiatelly and not when it will be required.
        /// </summary>
        public void CreateAndCacheInstance()
        {
            lock (this)
            {
                throw new NotImplementedException();
            }
        }
    }
}
