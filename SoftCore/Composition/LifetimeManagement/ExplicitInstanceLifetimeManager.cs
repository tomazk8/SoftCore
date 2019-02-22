using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition.LifetimeManagement
{
    /// <summary>
    /// Used when explicit instance is registered into the catalog. Since catalog cannot contain
    /// instances themselves, the instance is provided through this instance factory in the
    /// CompositionPart.
    /// </summary>
    public class ExplicitInstanceLifetimeManager : LifetimeManager
    {
        private object instance;

        public ExplicitInstanceLifetimeManager(object instance)
        {
            this.instance = instance;
        }

        public override InstanceInfo GetInstance(IEnumerable<object> args)
        {
            return new InstanceInfo(instance, false);
        }
    }
}
