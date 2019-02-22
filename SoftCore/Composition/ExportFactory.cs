using System;
using System.Collections.Generic;
using System.Text;

namespace SoftCore.Composition
{
    public class ExportFactory<T>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport()
        {
            return (T)export.ToInstance();
        }
    }

    public class ExportFactory<T, TParam1>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1)
        {
            return (T)export.ToInstance(new object[] { param1 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2)
        {
            return (T)export.ToInstance(new object[] { param1, param2 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2, TParam3>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return (T)export.ToInstance(new object[] { param1, param2, param3 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2, TParam3, TParam4>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return (T)export.ToInstance(new object[] { param1, param2, param3, param4 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return (T)export.ToInstance(new object[] { param1, param2, param3, param4, param5 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            return (T)export.ToInstance(new object[] { param1, param2, param3, param4, param5, param6 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
        {
            return (T)export.ToInstance(new object[] { param1, param2, param3, param4, param5, param6, param7 });
        }
    }

    public class ExportFactory<T, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>
    {
        private Export export;

        public ExportFactory(Export export)
        {
            this.export = export;
        }

        public T CreateExport(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8)
        {
            return (T)export.ToInstance(new object[] { param1, param2, param3, param4, param5, param6, param7, param8 });
        }
    }
}
