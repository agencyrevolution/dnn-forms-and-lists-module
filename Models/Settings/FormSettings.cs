using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class FormSettings
    {
        [JsonProperty(PropertyName = "submissionText")]
        public string SubmissionText { get; set; }

        [JsonProperty(PropertyName = "uponSubmitRedirect")]
        public string UponSubmitRedirect { get; set; }

        [JsonProperty(PropertyName = "uponSubmitAction")]
        public string UponSubmitAction { get; set; }
    }
}