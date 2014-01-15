using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class EmailSettings
    {
        [JsonProperty(PropertyName = "trackingScript")]
        public string TrackingScripts { get; set; }

        [JsonProperty(PropertyName = "trackingEmailTo")]
        public string TrackingEmailTo { get; set; }

        [JsonProperty(PropertyName = "trackingEmailCc")]
        public string TrackingEmailCc { get; set; }

        [JsonProperty(PropertyName = "trackingEmailBcc")]
        public string TrackingEmailBcc { get; set; }

        [JsonProperty(PropertyName = "trackingEmailReplyTo")]
        public string TrackingEmailReplyTo { get; set; }

        [JsonProperty(PropertyName = "trackingEmailFrom")]
        public string TrackingEmailFrom { get; set; }

        [JsonProperty(PropertyName = "trackingSubject")]
        public string TrackingSubject { get; set; }

        [JsonProperty(PropertyName = "trackingMessage")]
        public string TrackingMessage { get; set; }

        [JsonProperty(PropertyName = "trackingTriggerOnDelete")]
        public string TrackingTriggerOnDelete { get; set; }

        [JsonProperty(PropertyName = "trackingTriggerOnNew")]
        public string TrackingTriggerOnNew { get; set; }

        [JsonProperty(PropertyName = "trackingTriggerOnUpdate")]
        public string TrackingTriggerOnUpdate { get; set; }

        [JsonProperty(PropertyName = "trackingTextOnNew")]
        public string TrackingTextOnNew { get; set; }

        [JsonProperty(PropertyName = "trackingTextOnUpdate")]
        public string TrackingTextOnUpdate { get; set; }

        [JsonProperty(PropertyName = "trackingTextOnDelete")]
        public string TrackingTextOnDelete { get; set; }

    }
}