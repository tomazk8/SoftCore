using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public class Export
    {
        private Func<IEnumerable<object>, object> instanceCreator;

        public Export(Func<IEnumerable<object>, object> instanceCreator)
        {
            this.instanceCreator = instanceCreator;
        }

        public object ToInstance()
        {
            return instanceCreator(null);
        }
        public object ToInstance(IEnumerable<object> args)
        {
            return instanceCreator(args);
        }
    }
}
