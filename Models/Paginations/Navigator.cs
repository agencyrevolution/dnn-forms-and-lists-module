using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.Paginations
{
    public class Navigator : PageBase
    {
        [JsonProperty(PropertyName = "disabled")]
        public bool Disabled { get; set; }
    }

}