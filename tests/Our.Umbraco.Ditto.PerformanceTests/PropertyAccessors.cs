using System.ComponentModel;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Our.Umbraco.Ditto.PerformanceTests
{
    public class PropertyAccessors
    {
        private PropertyAccessors obj;

        private dynamic dlr;
        private PropertyInfo prop;
        private PropertyDescriptor descriptor;

        public string Value { get; set; }

        [Setup]
        public void Setup()
        {
            this.obj = new PropertyAccessors();
            this.dlr = this.obj;
            this.prop = typeof(PropertyAccessors).GetProperty("Value");
            this.descriptor = TypeDescriptor.GetProperties(this.obj)["Value"];
        }

        [Benchmark(Description = "1. Static C#", Baseline = true)]
        public string StaticCSharp()
        {
            this.obj.Value = "abc";
            return this.obj.Value;
        }

        [Benchmark(Description = "2. Dynamic C#")]
        public string DynamicCSharp()
        {
            this.dlr.Value = "abc";
            return this.dlr.Value;
        }

        [Benchmark(Description = "3. PropertyInfo")]
        public string PropertyInfo()
        {
            this.prop.SetValue(this.obj, "abc", null);
            return (string)this.prop.GetValue(this.obj, null);
        }

        [Benchmark(Description = "4. PropertyDescriptor")]
        public string PropertyDescriptor()
        {
            this.descriptor.SetValue(this.obj, "abc");
            return (string)this.descriptor.GetValue(this.obj);
        }

        [Benchmark(Description = "5. PropertyInfoInvocations")]
        public string PropertyInvocations()
        {
            PropertyInfoInvocations.SetValue(this.prop, this.obj, "abc");
            return (string)PropertyInfoInvocations.GetValue(this.prop, this.obj);
        }
    }
}