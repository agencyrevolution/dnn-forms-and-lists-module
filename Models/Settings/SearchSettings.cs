using System;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Settings
{
    [Serializable]
    public class SearchSettings
    {
        [JsonProperty(PropertyName = "excludeFromSearch")]
        public string ExcludeFromSearch { get; set; }

        [JsonProperty(PropertyName = "sortOrder")]
        public string SortOrder { get; set; }

        [JsonProperty(PropertyName = "sortField")]
        public string SortField { get; set; }

        [JsonProperty(PropertyName = "paging")]
        public string Paging { get; set; }

        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; set; }

        [JsonProperty(PropertyName = "topCount")]
        public string TopCount { get; set; }

        [JsonProperty(PropertyName = "showSearchTextBox")]
        public string ShowSearchTextBox { get; set; }

        [JsonProperty(PropertyName = "showNoRecordsUntilSearch")]
        public string ShowNoRecordsUntilSearch { get; set; }

        [JsonProperty(PropertyName = "simpleSearch")]
        public string SimpleSearch { get; set; }

        [JsonProperty(PropertyName = "urlSearch")]
        public string UrlSearch { get; set; }
    }
}