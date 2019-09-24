using NUnit.Framework;
using SoftCore;
using SoftCore.Composition;
using SoftCore.Interception;
using System.Collections.Generic;
using System.Security;

namespace SoftCore.Interception.UnitTests
{
    public class AccessControlDemonstrationTest
    {
        private AccessControlList accessControlList;

        [SetUp]
        public void Setup()
        {
            accessControlList = new AccessControlList();
        }

        [Test]
        public void TestAccessControlForUser()
        {
            accessControlList.Clear();
            accessControlList.AllowCall(nameof(ApiServer.GetName));

            TypeCatalog catalog = new TypeCatalog(
                typeof(ApiClient),
                typeof(ApiServer));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            CompositeApplicationInterceptor interceptor = new CompositeApplicationInterceptor(compositeApplication);
            interceptor.CallIntercepting += Interceptor_CallIntercepted;

            var api = compositeApplication.GetExportedValue<ApiClient>();

            string name = api.GetName();

            // User can get the name
            Assert.AreEqual(name, "Nada");

            // User cannot change the name. SecurityException must be thrown
            SecurityException se = Assert.Catch(() => api.SetName("Maria")) as SecurityException;
            Assert.NotNull(se);

            // Name must not change
            name = api.GetName();
            Assert.AreEqual(name, "Nada");
        }

        [Test]
        public void TestAccessControlForAdmin()
        {
            accessControlList.Clear();
            accessControlList.AllowCall(nameof(ApiServer.GetName));
            accessControlList.AllowCall(nameof(ApiServer.SetName));

            TypeCatalog catalog = new TypeCatalog(
                typeof(ApiClient),
                typeof(ApiServer));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            CompositeApplicationInterceptor interceptor = new CompositeApplicationInterceptor(compositeApplication);
            interceptor.CallIntercepting += Interceptor_CallIntercepted;

            var api = compositeApplication.GetExportedValue<ApiClient>();

            string name = api.GetName();

            // User can get the name
            Assert.AreEqual(name, "Nada");

            // User cannot change the name. SecurityException must be thrown
            Assert.DoesNotThrow(() => api.SetName("Maria"));

            // Name must not change
            name = api.GetName();
            Assert.AreEqual(name, "Maria");
        }

        public class AccessControlList
        {
            List<string> allowedMethods = new List<string>();

            public void AllowCall(string methodName)
            {
                allowedMethods.Add(methodName);
            }
            public void Clear()
            {
                allowedMethods.Clear();
            }
            public void CheckCall(string methodName)
            {
                if (!allowedMethods.Contains(methodName))
                    throw new SecurityException("User is not allowed to call the method " + methodName);
            }
        }

        private void Interceptor_CallIntercepted(object sender, CallInterceptingEventArgs e)
        {
            accessControlList.CheckCall(e.MemberInvocation.Method.Name);
        }

        #region Classes
        [Export]
        public class ApiClient
        {
            [Import]
            private IApiServer apiServer;

            private ApiClient()
            {
            }

            public string GetName()
            {
                // Simulate a call to server API
                return apiServer.GetName();
            }
            public void SetName(string name)
            {
                // Simulate a call to server API
                apiServer.SetName(name);
            }
        }

        public interface IApiServer
        {
            string GetName();
            void SetName(string name);
        }

        [Export(typeof(IApiServer))]
        public class ApiServer : IApiServer
        {
            private string name = "Nada";

            private ApiServer()
            {
            }

            public string GetName()
            {
                return name;
            }
            public void SetName(string name)
            {
                this.name = name;
            }
        }
        #endregion
    }
}