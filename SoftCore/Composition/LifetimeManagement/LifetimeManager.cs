using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public abstract class LifetimeManager
    {
        /// <summary>
        /// Gets an instance of the specified type. Depending on the lifetime of the specified type, this
        /// method could create a new instance or return a previously created instance. It is very
        /// important that when a new instance is created in this method, CreatingInstance event
        /// is trigerred. Failing to do so will result in imports not being set.
        /// </summary>
        public abstract InstanceInfo GetInstance(IEnumerable<object> args);
    }

    public struct InstanceInfo
    {
        public InstanceInfo(object instance, bool isNewInstance)
        {
            this.Instance = instance;
            this.IsNewInstance = isNewInstance;
        }

        public readonly object Instance;
        /// <summary>
        /// This field is very important in the composition. Since lifetime managers can return new or
        /// existing instance, the library must get this information. If set to true, all imports
        /// will be satisfied and <see cref="IPartSatisfiedCallback.OnImportsSatisfied"/> method will be invoked
        /// on the instance. When false, instance will be consider alreadx satisfied.
        /// </summary>
        public readonly bool IsNewInstance;
    }
}
