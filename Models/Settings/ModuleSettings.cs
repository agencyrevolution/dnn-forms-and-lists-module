using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class ModuleSettings
    {
        [JsonProperty(PropertyName = "editOnlyOwnItems")]
        public string EditOnlyOwnItems { get; set; }

        [JsonProperty(PropertyName = "editPrivateColumnsForAdmins")]
        public string EditPrivateColumnsForAdmins { get; set; }

        [JsonProperty(PropertyName = "forceCaptchaForAnonymous")]
        public string ForceCaptchaForAnonymous { get; set; }

        [JsonProperty(PropertyName = "forceInputFiltering")]
        public string ForceInputFiltering { get; set; }

        [JsonProperty(PropertyName = "showAllColumnsForAdmins")]
        public string ShowAllColumnsForAdmins { get; set; }

        [JsonProperty(PropertyName = "showSystemColumns")]
        public string ShowSystemColumns { get; set; }

        [JsonProperty(PropertyName = "userRecordQuota")]
        public string UserRecordQuota { get; set; }
    }
}