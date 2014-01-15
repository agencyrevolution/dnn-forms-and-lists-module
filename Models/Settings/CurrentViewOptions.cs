using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class CurrentViewOptions
    {
        [JsonProperty(PropertyName = "appearance")]
        public string Appearance { get; set; }

        [JsonProperty(PropertyName = "useButtonsInForm")]
        public string UseButtonsInForm { get; set; }
    }
}