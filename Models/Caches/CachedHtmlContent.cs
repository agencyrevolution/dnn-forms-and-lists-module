using System;
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Caches
{
    [TableName("UserDefinedCachedHtmlContents")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [Scope("ModuleId")]
    [Cacheable("UserDefinedCachedHtmlContents", CacheItemPriority.Default, 20)]
    public class CachedHtmlContent
    {
        #region Constants

        public const string ModuleIdProperty = "ModuleId";

        public const string KeywordProperty = "Keyword";

        public const string SkipProperty = "Skip";

        public const string LimitProperty = "Limit";

        #endregion

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "moduleId")]
        public int ModuleId { get; set; }

        [JsonProperty(PropertyName = "keyword")]
        public string Keyword { get; set; }

        [JsonProperty(PropertyName = "skip")]
        public int Skip { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int Limit { get; set; }

        [JsonProperty(PropertyName = "htmlContent")]
        public string HtmlContent { get; set; }

        [JsonProperty(PropertyName = "lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }
    }
}