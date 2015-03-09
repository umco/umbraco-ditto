using Newtonsoft.Json;

namespace Our.Umbraco.Ditto.Models.Archetype
{
    public class PropertyModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}