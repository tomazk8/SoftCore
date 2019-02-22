using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoftCore.Features
{
    /*public abstract class MethodCallInterceptor
    {
        /// <summary>
        /// Called when a method on an interface is called.
        /// </summary>
        public abstract void Intercept(MethodCallInfo methodCallInfo);
    }

    /// <summary>
    /// Encapsulates an invocation of a proxied method. Copyed from Castle.Proxy IInvocation
    /// </summary>
    public class MethodCallInfo
    {
        public MethodCallInfo(IInvocation invocation)
        {
            Arguments = invocation.Arguments != null ? invocation.Arguments.ToArray() : null;
            GenericArguments = invocation.GenericArguments != null ? invocation.GenericArguments.ToArray() : null;
            InvocationTarget = invocation.InvocationTarget;
            Method = invocation.Method;
            MethodInvocationTarget = invocation.MethodInvocationTarget;
            Proxy = invocation.Proxy;
        }


        /// <summary>
        /// Gets the arguments that the method has been invoked with.
        /// </summary>
        public object[] Arguments { get; private set; }
        /// <summary>
        /// Gets the generic arguments of the method.
        /// </summary>
        public Type[] GenericArguments { get; private set; }
        /// <summary>
        /// Gets the object on which the invocation is performed. This is different from proxy object because most
        /// of the time this will be the proxy target object.
        /// </summary>
        public object InvocationTarget { get; private set; }
        /// <summary>
        /// Gets the System.Reflection.MethodInfo representing the method being invoked on the proxy.
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// For interface proxies, this will point to the System.Reflection.MethodInfo on the target class.
        /// </summary>
        public MethodInfo MethodInvocationTarget { get; private set; }
        /// <summary>
        /// Gets the proxy object on which the intercepted method is invoked.
        /// </summary>
        public object Proxy { get; private set; }
    }*/
}
