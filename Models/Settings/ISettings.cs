using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class SettingsBase
    {
        [JsonProperty(PropertyName = "moduleSettings")] 
        public ModuleSettings ModuleSettings { get; set; }

        [JsonProperty(PropertyName = "moduleConfiguration")] 
        public ModuleConfiguration ModuleConfiguration { get; set; }
    }
}