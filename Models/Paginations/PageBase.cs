using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Paginations
{
    public class PageBase
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}