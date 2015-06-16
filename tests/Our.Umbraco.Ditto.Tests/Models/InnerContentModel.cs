using System.Collections.Generic;
using System.Globalization;

namespace Our.Umbraco.Ditto.Tests.Models
{
    public class InnerContentModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public InnerContentModel2 ContentProp { get; set; }

        public IEnumerable<InnerContentModel2> ContentListProp { get; set; }
    }

    public class InnerContentModel2
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string InnerProp { get; set; }
    }
}