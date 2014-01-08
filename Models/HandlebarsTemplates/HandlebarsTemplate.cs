/*
' Copyright (c) 2013 Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Modules.UserDefinedTable.Helpers;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable.Models.HandlebarsTemplates
{
    [TableName("UserDefinedHandlebarsTemplates")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [Scope("ModuleId")]
    [Cacheable("UserDefinedHandlebarsTemplates", CacheItemPriority.Default, 20)]
    public class HandlebarsTemplate
    {
        ///<summary>
        /// The ID of your object with the name of the ItemName
        ///</summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Each template belongs to a specific module
        /// </summary>
        [JsonProperty(PropertyName = "moduleId")]
        public int ModuleId { get; set; }

        ///<summary>
        /// The string content follows Handlebars template syntax
        ///</summary>
        [JsonProperty(PropertyName = "templateString")]
        public string TemplateString { get; set; }

        ///<summary>
        /// An integer for the user id of the user who created the object
        ///</summary>
        [JsonProperty(PropertyName = "createdByUserId")]
        public int CreatedByUserId { get; set; }

        ///<summary>
        /// An integer for the user id of the user who last updated the object
        ///</summary>
        [JsonProperty(PropertyName = "lastModifiedByUserId")]
        public int LastModifiedByUserId { get; set; }

        ///<summary>
        /// The date the object was created
        ///</summary>
        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        ///<summary>
        /// The date the object was updated
        ///</summary>
        [JsonProperty(PropertyName = "lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }

        ///<summary>
        /// Contains the list of required JavaScript to be rendered on View page.
        /// Separate by , or ; or | and trim start and end any spaces.
        ///</summary>
        [JsonProperty(PropertyName = "requiredJavaScripts")]
        public string RequiredJavaScripts { get; set; }

        ///<summary>
        /// Contains the list of required css files to be rendered on View page.
        /// Separate by , or ; or | and trim start and end any spaces.
        ///</summary>
        [JsonProperty(PropertyName = "requiredStylesheets")]
        public string RequiredStylesheets { get; set; }

        [IgnoreColumn]
        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy
        {
            get { return UserInfoHelpers.GetDisplayNameById(CreatedByUserId); }
        }

        [IgnoreColumn]
        [JsonProperty(PropertyName = "lastModifiedBy")]
        public string LastModifiedBy
        {
            get { return UserInfoHelpers.GetDisplayNameById(LastModifiedByUserId); }
        }
    }
}
