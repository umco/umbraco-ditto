using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Our.Umbraco.Ditto.Models.Archetype;
using Our.Umbraco.Ditto.Models.Archetype.Abstract;

namespace Our.Umbraco.Ditto.Tests.Extensions
{
    [TestFixture]
    public class TypeInferenceExtensionsTests
    {
        [TestCase("TestClass", Result = true, TestName = "Get type from base type and string of classname")]
        public bool FindClassesInAssembly(string name)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var result = TypeInferenceExtensions.GetTypeByName(name, typeof (ArchetypeModel)).Any();

            stopwatch.Stop();

            Console.WriteLine("took {0}ms to complete", stopwatch.ElapsedMilliseconds);

            return result;
        }
    }
}
