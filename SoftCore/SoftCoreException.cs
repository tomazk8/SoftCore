using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore
{
    public class SoftCoreException : Exception
    {
        public SoftCoreException(string message)
            : base(message)
        {
        }
    }
}
