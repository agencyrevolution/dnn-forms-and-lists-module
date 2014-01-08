using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Paginations
{
    public class Pagination
    {
        [JsonProperty(PropertyName = "previous")]
        public Navigator Previous { get; set; }

        [JsonProperty(PropertyName = "pages")]
        public IList<Page> Pages { get; set; }

        [JsonProperty(PropertyName = "next")]
        public Navigator Next { get; set; }

        [JsonProperty(PropertyName = "totalCount")]
        public int TotalCount { get; set; }

        public Pagination()
        {
            Pages = new List<Page>();
        }
    }
}