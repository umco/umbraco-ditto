using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Ditto.Tests.Models
{
    [UmbracoProperties(Prefix = "Site", Recursive = true)]
    public class PrefixedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Fallback { get; set; }

        [UmbracoProperty("UnprefixedProp")]
        public string UnprefixedProp { get; set; }
    }
}
