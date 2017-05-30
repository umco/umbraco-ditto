using System;
using System.IO;
using System.Reflection;

namespace Our.Umbraco.Ditto.Extensions.Internal
{
    internal static class AssemblyExtensions
    {
        /// <summary>
        /// Private var determining if we have a loadable app code assembly. We only want to do this once
        /// per app recyle so we make it static and lazy so that it only populates when used.
        /// </summary>
        private static readonly Lazy<bool> HasLoadableAppCodeAssembly = new Lazy<bool>(() =>
        {
            try
            {
                Assembly.Load("App_Code");
                return true;
            }
            catch (FileNotFoundException)
            {
                //this will occur if it cannot load the assembly
                return false;
            }
        });

        /// <summary>
        /// Returns true if the assembly is the App_Code assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <remarks>Taken from Umbraco.Core.AssemblyExtensions</remarks>
        public static bool IsAppCodeAssembly(this Assembly assembly)
        {
            return assembly.FullName.StartsWith("App_Code") && HasLoadableAppCodeAssembly.Value;
        }
    }
}