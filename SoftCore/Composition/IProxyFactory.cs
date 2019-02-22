using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SoftCore.Composition
{
    public interface IProxyFactory
    {
        /// <summary>
        /// Creates a proxy of specified type.
        /// </summary>
        /// <param name="typeInfo">The type of proxy to create</param>
        /// <param name="originalInstanceGetter">Used to retrieve the original instance when wrapping it. Proxy can be used without original instance.</param>
        /// <returns></returns>
        object CreateProxy(TypeInfo typeInfo, Func<object> originalInstanceGetter);
    }
}
