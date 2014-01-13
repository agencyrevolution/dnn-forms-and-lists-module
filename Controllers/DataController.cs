using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Controllers.Caches;
using DotNetNuke.Modules.UserDefinedTable.Models.Caches;
using DotNetNuke.Modules.UserDefinedTable.Models.Paginations;
using DotNetNuke.Modules.UserDefinedTable.Models.SearchOptions;
using DotNetNuke.Security.Permissions;
using Newtonsoft.Json;
using Rev.RestClients;

namespace DotNetNuke.Modules.UserDefinedTable.Controllers
{
    public class DataController
    {
        /// <summary>
        /// Search data by options, return json string
        /// </summary>
        /// <param name="options"></param>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public string GetJsonString(SearchOptions options, string pageUrl)
        {
            // data set
            var dataSet = GetDataSet(options);
            if (dataSet == null)
                return null;

            // filter data rows
            var totalCount = FilterDataRow(options, dataSet);

            // normalized json
            var json = JsonConvert.SerializeObject(dataSet);
            var normalizedJson = JsonHelpers.NormalizeJson(json);

            // pagination?
            if (options.Pagination)
                normalizedJson = AddPaginationJson(options, totalCount, normalizedJson);

            return normalizedJson;
        }

        /// <summary>
        /// Search data by options, return Html content
        /// </summary>
        /// <param name="options"></param>
        /// <param name="allowAnonymous"></param>
        /// <returns></returns>
        public string GetHtml(SearchOptions options, bool allowAnonymous = false)
        {
            // look for cache first
            var cachedHtmlContent = new CachedHtmlContentController().GetCachedHtmlContent(options.ModuleId,
                options.Skip, options.Limit, options.IsEditMode, options.Keyword);
            if (cachedHtmlContent != null && !string.IsNullOrEmpty(cachedHtmlContent.HtmlContent))
                return cachedHtmlContent.HtmlContent;

            var dataSet = GetDataSet(options, allowAnonymous);
            if (dataSet == null)
                return null;

            // validate template
            if (string.IsNullOrEmpty(options.TemplateContent))
            {
                var template = new HandlebarsTemplateController().GetTemplateByModuleId(options.ModuleId);
                if (template == null)
                    throw new Exception("HandlebarsTemplate not found");
                if (string.IsNullOrEmpty(template.TemplateString))
                    throw new Exception("The HandlebarsTemplate content is null or empty");
                options.TemplateContent = template.TemplateString;
            }

            // fiter data rows
            var totalCount = FilterDataRow(options, dataSet);

            // normalize json
            var normalizedJson = JsonHelpers.NormalizeJson(JsonConvert.SerializeObject(dataSet));

            // pagination?
            if (options.Pagination)
                normalizedJson = AddPaginationJson(options, totalCount, normalizedJson);
            
            // html content from nodejs power template response
            var htmlContent = new TemplateRestClient().GetHtmlContent(options.TemplateContent, normalizedJson);

            // cache to database
            if (!string.IsNullOrEmpty(htmlContent))
                new CachedHtmlContentController().SaveCachedHtmlContent(new CachedHtmlContent
                {
                    Keyword = options.Keyword,
                    HtmlContent = htmlContent,
                    ModuleId = options.ModuleId,
                    LastModifiedDate = DateTime.Now,
                    Limit = options.Limit,
                    Skip = options.Skip,
                    IsEditMode = options.IsEditMode
                });
            return htmlContent;
        }

        #region Helpers

        /// <summary>
        /// Filter data rows
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private static int FilterDataRow(SearchOptions options, DataSet dataSet)
        {
            // filter by keyword
            FilterByKeyword(options.Keyword, dataSet.Tables[DataSetTableName.Data]);

            // keep total count
            var totalCount = dataSet.Tables[DataSetTableName.Data].Rows.Count;

            // skip & limit rows
            FilterBySkipAndLimit(options.Skip, options.Limit, dataSet.Tables[DataSetTableName.Data]);
            return totalCount;
        }

        /// <summary>
        /// Get DataSet
        /// </summary>
        /// <param name="options"></param>
        /// <param name="allowAnonymous"></param>
        /// <returns></returns>
        protected DataSet GetDataSet(SearchOptions options, bool allowAnonymous = false)
        {
            var userInfo = UserController.GetCurrentUserInfo();
            var mc = new ModuleController();
            var settings = mc.GetModuleSettings(options.ModuleId);
            var moduleInfo = mc.GetModule(options.ModuleId, options.TabId);

            if (allowAnonymous || ModulePermissionController.CanManageModule(moduleInfo))
            {
                var udt = new UserDefinedTableController(options.ModuleId, options.TabId, userInfo);
                var ds = udt.GetDataSet(true);

                // remove Fields and FieldSettings tables from dataset
                ds.Tables.Remove(DataSetTableName.FieldSettings);
                ds.Tables.Remove(DataSetTableName.Fields);

                // filter by keyword

                // add Context table to dataset
                ds.Tables.Add(udt.Context(moduleInfo, userInfo,
                                             options.Keyword,
                                             settings[SettingName.SortField].AsString(),
                                             settings[SettingName.SortOrder].AsString(),
                                             settings[SettingName.Paging].AsString()));

                return ds;
            }
            return null;
        }

        /// <summary>
        /// Filter by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="table"></param>
        protected static void FilterByKeyword(string keyword, DataTable table)
        {
            if (string.IsNullOrEmpty(keyword))
                return;

            var keywords = keyword.ToLower().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();

            // filter by keywords
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                if(!IsKeywordMatched(keywords, row, table.Columns.Count))
                    row.Delete();
            }
            table.AcceptChanges();
        }

        /// <summary>
        /// At least one keyword match at least one column data?
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="row"></param>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        protected static bool IsKeywordMatched(List<string> keywords, DataRow row, int columnCount)
        {
            for (var i = 0; i < columnCount; i++)
            {
                if (row[i] == null) continue;
                var data = row[i].ToString().ToLower();
                if (keywords.Any(data.Contains))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Filter by skip and limit
        /// </summary>
        protected static void FilterBySkipAndLimit(int skip, int limit, DataTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                if (i < skip || i >= skip + limit)
                    row.Delete();
            }
            table.AcceptChanges();
        }

        /// <summary>
        /// Add Pagination json to the normalized json string
        /// </summary>
        /// <param name="options"></param>
        /// <param name="totalCount"></param>
        /// <param name="normalizedJson"></param>
        /// <returns></returns>
        protected string AddPaginationJson(SearchOptions options, int totalCount, string normalizedJson)
        {
            // pagination
            var pagination = BuildPagination(options, totalCount);

            var paginationJson = JsonConvert.SerializeObject(pagination);
            if (normalizedJson.StartsWith("{"))
            {
                if (normalizedJson.StartsWith("{"))
                {
                    var lastIndex = normalizedJson.LastIndexOf("}", StringComparison.Ordinal);
                    return normalizedJson.Insert(lastIndex, string.Format(",\"pagination\":{0}", paginationJson));
                }
            }
            return normalizedJson;
        }

        /// <summary>
        /// Build Pagination from search options, module info and main data table
        /// </summary>
        /// <param name="options"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        protected Pagination BuildPagination(SearchOptions options, int totalCount)
        {
            var pagination = new Pagination {TotalCount = totalCount};


            // pages
            var pageCount = pagination.TotalCount / options.Limit + (pagination.TotalCount % options.Limit > 0 ? 1 : 0);
            var activePageIndex = -1;
            for (int i = 0; i < pageCount; i++)
            {
                var page = new Page
                {
                    PageIndex = i + 1,
                    Url =
                        string.Format("{0}{1}", FriendlyUrlHelpers.GetFriendlyUrl(options.ModuleId),
                            BuildQueryString(options.Keyword, i * options.Limit, options.Limit)),
                    Active = options.Skip < (i + 1) * options.Limit && options.Skip >= i * options.Limit
                };

                if (page.Active)
                    activePageIndex = i;

                pagination.Pages.Add(page);
            }

            // previous navigator
            if (activePageIndex > 0)
            {
                pagination.Previous = new Navigator
                {
                    Url = pagination.Pages[activePageIndex - 1].Url,
                    Disabled = false
                };
            }
            else
            {
                pagination.Previous = new Navigator
                {
                    Url = string.Empty,
                    Disabled = true
                };
            }

            // next navigator
            if (activePageIndex < pageCount - 1)
            {
                pagination.Next = new Navigator
                {
                    Url = pagination.Pages[activePageIndex + 1].Url,
                    Disabled = false
                };
            }
            else
            {
                pagination.Next = new Navigator
                {
                    Url = string.Empty,
                    Disabled = true
                };
            }

            return pagination;
        }

        /// <summary>
        /// Build query string from keyword, skip and limit
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        protected string BuildQueryString(string keyword, int skip, int limit)
        {
            return string.IsNullOrEmpty(keyword)
                ? string.Format("?skip={0}&limit={1}", skip, limit)
                : string.Format("?skip={0}&limit={1}&k={2}", skip, limit, keyword);
        }

        #endregion
    }
}