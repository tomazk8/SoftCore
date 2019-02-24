using System;
using System.Collections.Generic;
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
            
        }
    }
}
