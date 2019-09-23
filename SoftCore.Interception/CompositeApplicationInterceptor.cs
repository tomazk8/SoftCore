using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using SoftCore;

namespace SoftCore.Interception
{
    public class CompositeApplicationInterceptor
    {
        private ProxyGenerator proxyGenerator = new ProxyGenerator();
        private Interceptor interceptor;

        public CompositeApplicationInterceptor(CompositeApplication compositeApplication)
        {
            compositeApplication.SatisfyingImport += CompositeApplication_SatisfyingImport;
            compositeApplication.PreRunChecking += CompositeApplication_PreRunChecking;

            interceptor = new Interceptor(this);
        }

        internal void OnCallIntercepted(MemberInvocation memberInvocation)
        {
            CallIntercepted?.Invoke(this, new CallInterceptedEventArgs(memberInvocation));
        }

        private void CompositeApplication_PreRunChecking(object sender, Composition.PreRunCheckingEventArgs e)
        {
            // If an interface is imported for which a proxy will be created, this interface must not use events
            // because events cannot be handled by the proxy.
        }

        private void CompositeApplication_SatisfyingImport(object sender, Composition.SatisfyingImportEventArgs e)
        {
            // Create a proxy if field is interface and replace the instance with the proxy
            if (e.FieldInfo.FieldType.IsInterface)
            {
                e.Instance = proxyGenerator.CreateInterfaceProxyWithTarget(e.FieldInfo.FieldType, e.Instance, interceptor);
            }
        }

        public event EventHandler<CallInterceptedEventArgs> CallIntercepted;
    }

    public class CallInterceptedEventArgs : EventArgs
    {
        public CallInterceptedEventArgs(MemberInvocation memberInvocation)
        {
            this.MemberInvocation = memberInvocation;
        }

        public MemberInvocation MemberInvocation { get; private set; }
    }

    class Interceptor : IInterceptor
    {
        private CompositeApplicationInterceptor compositeApplicationInterceptor;

        public Interceptor(CompositeApplicationInterceptor compositeApplicationInterceptor)
        {
            this.compositeApplicationInterceptor = compositeApplicationInterceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            MemberInvocation memberInvocation = new MemberInvocation(invocation.Arguments, invocation.GenericArguments,
                invocation.Method, invocation.ReturnValue, invocation.TargetType);

            compositeApplicationInterceptor.OnCallIntercepted(memberInvocation);

            invocation.Proceed();
        }
    }

    public class MemberInvocation
    {
        public MemberInvocation(object[] arguments, Type[] genericArguments, MethodInfo method, 
            object returnValue, Type targetType)
        {
            this.Arguments = arguments;
            this.GenericArguments = genericArguments;
            this.Method = method;
            this.ReturnValue = returnValue;
            this.TargetType = targetType;
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
        /// Gets the System.Reflection.MethodInfo representing the method being invoked on the proxy.
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// Gets or sets the return value of the method.
        /// </summary>
        public object ReturnValue { get; set; }
        /// <summary>
        /// Gets the type of the target object for the intercepted method.
        /// </summary>
        public Type TargetType { get; private set; }
    }
}
