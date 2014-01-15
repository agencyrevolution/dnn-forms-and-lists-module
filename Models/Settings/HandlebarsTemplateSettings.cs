using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class HandlebarsTemplateSettings
    {
        [JsonProperty(PropertyName = "templateUrl")]
        public string TemplateUrl { get; set; }

        [JsonProperty(PropertyName = "templateContent")]
        public string TemplateContent { get; set; }

        [JsonProperty(PropertyName = "requiredJavaScripts")]
        public string RequiredJavaScripts { get; set; }

        [JsonProperty(PropertyName = "requiredStylesheets")]
        public string RequiredStylesheets { get; set; }
    }
}