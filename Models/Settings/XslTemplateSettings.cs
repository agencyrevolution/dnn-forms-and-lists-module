using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class XslTemplateSettings
    {
        [JsonProperty(PropertyName = "templateUrl")]
        public string TemplateUrl { get; set; }
        
        [JsonProperty(PropertyName = "templateContent")]
        public string TemplateContent { get; set; }
    }
}