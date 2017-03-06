using BenchmarkDotNet.Running;

namespace Our.Umbraco.Ditto.PerformanceTests
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">
        /// The arguments to pass to the program.
        /// </param>
        public static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).Assembly).Run(args);
        }
    }
}