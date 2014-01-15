using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class ListSettings
    {
        [JsonProperty(PropertyName = "renderingMethod")]
        public string RenderingMethod { get; set; }

        [JsonProperty(PropertyName = "searchSettings")]
        public SearchSettings SearchSettings { get; set; }

        [JsonProperty(PropertyName = "xslTemplateSettings")]
        public XslTemplateSettings XslTemplateSettings { get; set; }

        [JsonProperty(PropertyName = "handlebarsTemplateSettings")]
        public HandlebarsTemplateSettings HandlebarsTemplateSettings { get; set; }
    }
}