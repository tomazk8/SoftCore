using NUnit.Framework;
using SoftCore.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCore.Interception.UnitTests
{
    public class LoggingTests
    {
        private List<LogEntry> log = new List<LogEntry>();

        [Import]
        private IClassA classA;

        [Test]
        public void TestLogging()
        {
            TypeCatalog catalog = new TypeCatalog(
                typeof(ClassA),
                typeof(ClassB));

            CompositeApplication compositeApplication = new CompositeApplication(catalog);
            CompositeApplicationInterceptor interceptor = new CompositeApplicationInterceptor(compositeApplication);
            interceptor.CallIntercepted += Interceptor_CallIntercepted;

            compositeApplication.SatisfyImportsOnInstance(this);
            var result = classA.Convert(7);

            Assert.AreEqual(log.Count, 2);

            Assert.AreEqual(log[0].TargetType, typeof(ClassB));
            Assert.AreEqual(log[0].TargetMethodName, nameof(ClassB.ConvertInternal));
            Assert.IsTrue(log[0].ReturnValue is string);
            Assert.AreEqual(log[0].ReturnValue, "8");
            Assert.AreEqual(log[0].Parameters.Count(), 1);
            Assert.IsTrue(log[0].Parameters.Single().Value is int);
            Assert.AreEqual((int)log[0].Parameters.Single().Value, 7);

            Assert.AreEqual(log[1].TargetType, typeof(ClassA));
            Assert.AreEqual(log[1].TargetMethodName, nameof(ClassA.Convert));
            Assert.IsTrue(log[1].ReturnValue is string);
            Assert.AreEqual(log[1].ReturnValue, "8");
            Assert.AreEqual(log[1].Parameters.Count(), 1);
            Assert.IsTrue(log[1].Parameters.Single().Value is int);
            Assert.AreEqual((int)log[1].Parameters.Single().Value, 7);
        }

        private void Interceptor_CallIntercepted(object sender, CallInterceptedEventArgs e)
        {
            List<MethodParameter> methodParameters = new List<MethodParameter>();

            {
                var tmpMethodParamsList = e.MemberInvocation.Method.GetParameters();

                for (int i = 0; i < e.MemberInvocation.Arguments.Length; i++)
                {
                    methodParameters.Add(new MethodParameter
                    {
                        Name = tmpMethodParamsList[i].Name,
                        Value = e.MemberInvocation.Arguments[i]
                    });
                }
            }

            LogEntry logEntry = new LogEntry
            {
                TargetMethodName = e.MemberInvocation.Method.Name,
                TargetType = e.MemberInvocation.TargetType,
                Parameters = methodParameters,
                ReturnValue = e.MemberInvocation.ReturnValue
            };

            log.Add(logEntry);
        }

        #region Classes

        public class LogEntry
        {
            public string TargetMethodName;
            public Type TargetType;
            public IEnumerable<MethodParameter> Parameters;
            public object ReturnValue;
        }
        public class MethodParameter
        {
            public string Name;
            public object Value;
        }

        public interface IClassA
        {
            string Convert(int value);
        }

        [Export(typeof(IClassA))]
        public class ClassA : IClassA
        {
            [Import]
            private IClassB classB;

            private ClassA()
            {
            }

            public string Convert(int value)
            {
                return classB.ConvertInternal(value);
            }
        }

        public interface IClassB
        {
            string ConvertInternal(int value);
        }

        [Export(typeof(IClassB))]
        public class ClassB : IClassB
        {
            private ClassB()
            {
            }

            public string ConvertInternal(int value)
            {
                return (value + 1).ToString();
            }
        }
        #endregion
    }
}
