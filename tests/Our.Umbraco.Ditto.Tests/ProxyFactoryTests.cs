using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    [Category("Proxy Generation")]
    public class ProxyFactoryTests
    {
        [Test]
        public void ProxyCanGetSetProperties()
        {
            var factory = new ProxyFactory();
            IProxy proxy = factory.CreateProxy(typeof(TestClass), new List<string> { "id", "name" });
            proxy.Interceptor = new LazyInterceptor(new Dictionary<string, Lazy<object>>());

            // This is the method we are replicating within the property emitter.
            var tc = new TestClass();

            var idMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_Id", new[] { typeof(int) }).MethodHandle);

            var nameMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_Name", new[] { typeof(string) }).MethodHandle);

            var dateMethod =
                MethodBase.GetMethodFromHandle(typeof(TestClass).GetMethod("set_CreateDate", new[] { typeof(DateTime) }).MethodHandle);

            idMethod.Invoke(tc, new object[] { 1 });
            nameMethod.Invoke(tc, new object[] { "Foo" });
            dateMethod.Invoke(tc, new object[] { new DateTime(2017, 1, 1) });

            Assert.AreEqual(1, tc.Id);
            Assert.AreEqual("Foo", tc.Name);
            Assert.AreEqual(new DateTime(2017, 1, 1), tc.CreateDate);

            // ReSharper disable once SuspiciousTypeConversion.Global
            var testClass = (TestClass)proxy;

            testClass.Id = 1;
            testClass.Name = "Foo";
            testClass.CreateDate = new DateTime(2017, 1, 1);

            Assert.AreEqual(1, testClass.Id);
            Assert.AreEqual("Foo", testClass.Name);
            Assert.AreEqual(new DateTime(2017, 1, 1), testClass.CreateDate);
        }
    }

    public class TestClass
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
