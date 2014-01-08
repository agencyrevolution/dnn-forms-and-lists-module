using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.SearchOptions
{
    public class SearchOptions
    {
        public SearchOptions()
        {
            Skip = 0;
            Limit = 10;
        }

        [JsonProperty(PropertyName = "moduleId")]
        public int ModuleId { get; set; }

        [JsonProperty(PropertyName = "tabId")]
        public int TabId { get; set; }
        
        [JsonProperty(PropertyName = "skip")]
        public int Skip { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int Limit { get; set; }

        [JsonProperty(PropertyName = "keyword")]
        public string Keyword { get; set; }

        [JsonProperty(PropertyName = "pagination")]
        public bool Pagination { get; set; }

        [JsonProperty(PropertyName = "templateContent")]
        public string TemplateContent { get; set; }
    }
}