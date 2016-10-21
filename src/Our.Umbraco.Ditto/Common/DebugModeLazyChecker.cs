using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>A resettable lazy check of the ditto debug mode</summary>
    public class DebugModeLazyChecker : ResettableLazy<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugModeLazyChecker"/> class.
        /// </summary>
        public DebugModeLazyChecker() : base(CheckDebug)
        {

        }

        private static bool CheckDebug()
        {
            // Check for app setting first
            if (!ConfigurationManager.AppSettings["Ditto:DebugEnabled"].IsNullOrWhiteSpace())
            {
                return ConfigurationManager.AppSettings["Ditto:DebugEnabled"].InvariantEquals("true");
            }

            // Check the HTTP Context
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.IsDebuggingEnabled;
            }

            // Go and get it from config directly
            var section = ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
            return section != null && section.Debug;
        }
    }
}