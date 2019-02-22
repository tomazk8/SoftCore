using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features
{
    public class Recording
    {
    }

    /// <summary>
    /// Prevents recording of data, i.e. password from login
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    public class PreventDataRecordingAttribute : Attribute
    {
    }
}
