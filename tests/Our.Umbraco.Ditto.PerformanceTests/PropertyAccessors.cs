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

        [Benchmark(Description = "Static C#", Baseline = true)]
        public string StaticCSharp()
        {
            this.obj.Value = "abc";
            return this.obj.Value;
        }

        [Benchmark(Description = "Dynamic C#")]
        public string DynamicCSharp()
        {
            this.dlr.Value = "abc";
            return this.dlr.Value;
        }

        [Benchmark(Description = "PropertyInfo")]
        public string PropertyInfo()
        {
            this.prop.SetValue(this.obj, "abc", null);
            return (string)this.prop.GetValue(this.obj, null);
        }

        [Benchmark(Description = "PropertyDescriptor")]
        public string PropertyDescriptor()
        {
            this.descriptor.SetValue(this.obj, "abc");
            return (string)this.descriptor.GetValue(this.obj);
        }

        [Benchmark(Description = "PropertyInfoInvocations")]
        public string PropertyInvocations()
        {
            PropertyInfoInvocations.SetValue(this.prop, this.obj, "abc");
            return (string)PropertyInfoInvocations.GetValue(this.prop, this.obj);
        }

        [Benchmark(Description = "FastPropertyAccessor")]
        public string FastProperties()
        {
            FastPropertyAccessor.SetValue(this.prop, this.obj, "abc");
            return (string)FastPropertyAccessor.GetValue(this.prop, this.obj);
        }
    }
}