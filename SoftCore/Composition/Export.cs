using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public class Export
    {
        private Func<IEnumerable<object>, object> instanceCreator;

        public Export(Func<IEnumerable<object>, object> instanceCreator, Type exportType)
        {
            this.instanceCreator = instanceCreator;
            this.ExportType = exportType;
        }

        public Type ExportType { get; private set; }

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
