using System;
using System.Web;
using DotNetNuke.Modules.UserDefinedTable.Controllers;
using DotNetNuke.Modules.UserDefinedTable.Models;
using DotNetNuke.Modules.UserDefinedTable.Models.SearchOptions;

namespace DotNetNuke.Modules.UserDefinedTable
{
    /// <summary>
    /// Summary description for ShowJson
    /// </summary>
    public class ShowJson : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (!(context.Request.IsAuthenticated))
                {
                    context.Response.Write("Not authorized");
                    return;
                }

                // get ModuleId
                int moduleId;
                if (string.IsNullOrEmpty(context.Request.QueryString[QueryString.ModuleId]) ||
                    !int.TryParse(context.Request.QueryString[QueryString.ModuleId], out moduleId))
                {
                    context.Response.Write("Module ID (mid) not defined from query string (e.g. &mid=integer)");
                    return;
                }
                
                // get TabId
                int tabId;
                if (string.IsNullOrEmpty(context.Request.QueryString[QueryString.TabId])
                    || !int.TryParse(context.Request.QueryString[QueryString.TabId], out tabId))
                {
                    context.Response.Write("Tab ID (tid) is not defined from query string (e.g. &tid=integer)");
                    return;
                }
                
                var searchOptions = new SearchOptions
                {
                    ModuleId = moduleId,
                    TabId = tabId,
                    Pagination = true
                };

                // keyword
                if (!string.IsNullOrEmpty(context.Request.QueryString[QueryString.Keyword]))
                    searchOptions.Keyword = context.Request.QueryString[QueryString.Keyword];

                // skip
                int skip;
                if (!string.IsNullOrEmpty(context.Request.QueryString[QueryString.Skip]) &&
                    int.TryParse(context.Request.QueryString[QueryString.Skip], out skip))
                    searchOptions.Skip = skip;

                // limit
                int limit;
                if (!string.IsNullOrEmpty(context.Request.QueryString[QueryString.Limit]) &&
                    int.TryParse(context.Request.QueryString[QueryString.Limit], out limit))
                    searchOptions.Limit = limit;

                context.Response.ContentType = "application/json";
                var json = new DataController().GetJsonString(searchOptions, context.Request.Url.LocalPath);
                context.Response.Write(json);

            }
            catch (Exception ex)
            {
                var message = new
                {
                    status = "Error",
                    message = ex.Message,
                    stackStrace = ex.StackTrace,
                    source = ex.Source
                };
                context.Response.ContentType = "application/json";
                context.Response.Write(message);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}