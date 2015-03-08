using Newtonsoft.Json;

namespace Our.Umbraco.Ditto.Models.Archetype
{
    public class ArchetypeCMSModel
    {
        [JsonProperty("fieldsets")]
        public Fieldset[] Fieldsets { get; set; }
    }
}
