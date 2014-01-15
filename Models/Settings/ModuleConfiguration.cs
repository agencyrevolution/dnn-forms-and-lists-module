using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class ModuleConfiguration
    {
        [JsonProperty(PropertyName = "currentViewOptions")]
        public CurrentViewOptions CurrentViewOptions { get; set; }

        [JsonProperty(PropertyName = "formSettings")]
        public FormSettings FormSettings { get; set; }

        [JsonProperty(PropertyName = "listSettings")]
        public ListSettings ListSettings { get; set; }

        [JsonProperty(PropertyName = "emailSettings")]
        public EmailSettings EmailSettings { get; set; }
    }
}