using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Paginations
{
    public class Page : PageBase
    {
        [JsonProperty(PropertyName = "pageIndex")]
        public int PageIndex { get; set; }

        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }
    }
}