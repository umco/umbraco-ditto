using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Reflection;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace Our.Umbraco.Ditto.PerformanceTests
{


    public class Program
    {
        public class DittoPerformance
        {
            private DittoPerformance obj;

            private dynamic dlr;
            private PropertyInfo prop;
            private PropertyDescriptor descriptor;

            public string Value { get; set; }

            public static void Main(string[] args)
            {
                var summary = BenchmarkRunner.Run<DittoPerformance>(new Config());
                Console.WriteLine();

                // Display a summary to match the output of the original Performance test
                foreach (var report in summary.Reports.OrderBy(r => r.Key.Target.MethodTitle))
                {
                    Console.WriteLine("{0}: {1:N2} ns", report.Key.Target.MethodTitle, report.Value.ResultStatistics.Median);
                }

                Console.WriteLine();
            }

            [Setup]
            public void Setup()
            {
                this.obj = new DittoPerformance();
                this.dlr = this.obj;
                this.prop = typeof(DittoPerformance).GetProperty("Value");
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

        // BenchmarkDotNet settings (you can use the defaults, but these are tailored for this benchmark)
        private class Config : ManualConfig
        {
            public Config()
            {
                this.Add(Job.Default.WithLaunchCount(1));
                this.Add(PropertyColumn.Method);
                this.Add(StatisticColumn.Median, StatisticColumn.StdDev);
                this.Add(BaselineDiffColumn.Scaled);
                this.Add(CsvExporter.Default, MarkdownExporter.Default, MarkdownExporter.GitHub);
                this.Add(new ConsoleLogger());
            }
        }
    }
}
