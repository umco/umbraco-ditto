using Newtonsoft.Json;

namespace Our.Umbraco.Ditto.Models.Archetype
{
    public class Fieldset
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("properties")]
        public PropertyModel[] Properties { get; set; }
    }
}