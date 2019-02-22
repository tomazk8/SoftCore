using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    public class NonSharedLifetimeManager : LifetimeManager
    {
        private Type partType;

        public NonSharedLifetimeManager(Type paryType)
        {
            this.partType = paryType;
        }

        public override InstanceInfo GetInstance(IEnumerable<object> args)
        {
            // Always create a new instance. Exported classes must always have max one constructor.
            var constructor = partType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault();

            object instance = constructor != null
                ? constructor.Invoke(args != null ? args.ToArray() : Array.Empty<object>())
                : Activator.CreateInstance(partType);

            return new InstanceInfo(instance, true);
        }
    }
}
