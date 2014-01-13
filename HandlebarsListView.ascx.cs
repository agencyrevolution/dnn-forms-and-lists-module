using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Controllers;
using DotNetNuke.Modules.UserDefinedTable.Models;
using DotNetNuke.Modules.UserDefinedTable.Models.SearchOptions;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Modules.UserDefinedTable
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The UserDefinedTable Class provides the UI for displaying the UserDefinedTable
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class HandlebarsListView : PortalModuleBase
    {
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.Load += Page_Load;
            btnSearch.Click += btnSearch_Click;
        }

        /// <summary>
        /// Triggered when the user clicks on Search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
                Response.Redirect(string.Format("{0}?{1}={2}", FriendlyUrlHelpers.GetFriendlyUrl(ModuleId),
                    HttpUtility.UrlEncode(QueryString.Keyword), HttpUtility.UrlEncode(txtSearch.Text)));
            else
                Response.Redirect(FriendlyUrlHelpers.GetFriendlyUrl(ModuleId));
        }

        /// <summary>
        /// Triggered when the page is loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            try
            {
                // show or hide search box
                SetSearchBoxVisibility();

                // search options from query string
                var searchOptions = BuildSearchOptions();

                // template content
                var templateContent = GetTemplateContent();
                if (string.IsNullOrEmpty(templateContent))
                    return;

                searchOptions.TemplateContent = templateContent;

                // required JavaScripts
                RegisterJavaScripts();

                // required Stylesheets
                RegisterStylesheets();

                // get html from cache or nodejs power template engine
                var html = new DataController().GetHtml(searchOptions, true);

                // output to the main view
                placeHolderMainView.Controls.Clear();
                placeHolderMainView.Controls.Add(new LiteralControl(html));
            }
            catch (Exception exc) //Module failed to load
            {
                if (Request.IsAuthenticated)
                    Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Template content from settings
        /// </summary>
        /// <returns></returns>
        string GetTemplateContent()
        {
            if (ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateUrl))
            {
                var url = ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateUrl].AsString();
                if (string.IsNullOrEmpty(url))
                {
                    Exceptions.ProcessModuleLoadException("Handlebars template url is not defined. Please get it configured!", this, new Exception());
                    return null;
                }

                var file = FileManager.Instance.GetFile(ModuleContext.PortalId, url);
                if (file == null)
                {
                    Exceptions.ProcessModuleLoadException(string.Format("DNN file not found. Url: {0}", url), this, new Exception());
                    return null;
                }

                return HandlebarsTemplatesFile.ReadTemplateContentFromFile(file.FileId);
            }

            Exceptions.ProcessModuleLoadException("Handlebars template url is not defined. Please get it configured!", this, new Exception());
            return null;
        }

        /// <summary>
        /// Show or hide search box
        /// </summary>
        private void SetSearchBoxVisibility()
        {
            var settings = new ModuleController().GetTabModuleSettings(TabModuleId);
            bool showSearchbox;
            if (settings.ContainsKey(SettingName.ShowSearchTextBox) &&
                bool.TryParse(settings[SettingName.ShowSearchTextBox].ToString(), out showSearchbox))
                panelSearch.Visible = showSearchbox;
            else
                panelSearch.Visible = false;
            txtSearch.Text = Request.QueryString[QueryString.Keyword];
        }

        /// <summary>
        /// Register JavaScripts
        /// </summary>
        private void RegisterJavaScripts()
        {
            if (!ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts))
                return;

            var requiredJavaScripts =
                ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts].AsString();
            if (string.IsNullOrEmpty(requiredJavaScripts))
                return;

            var jsFiles = requiredJavaScripts.Split(new[] { ',', ';', '|' },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(jsFile => jsFile.Trim());

            foreach (var url in jsFiles)
            {
                Page.ClientScript.RegisterClientScriptInclude(GetHashCode().ToString(CultureInfo.InvariantCulture), url);
            }
        }

        /// <summary>
        /// Register Stylesheets
        /// </summary>
        private void RegisterStylesheets()
        {
            if (!ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets))
                return;

            var requiredStylesheets =
                ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets].AsString();
            if (string.IsNullOrEmpty(requiredStylesheets))
                return;

            var cssLinks = requiredStylesheets.Split(new[] { ',', ';', '|' },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(jsFile => jsFile.Trim());

            foreach (var cssLink in cssLinks)
            {
                var cssFile = new LiteralControl
                {
                    Text = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", cssLink)
                };
                Page.Header.Controls.Add(cssFile);
            }
        }

        /// <summary>
        /// Build SearchOptions from Request.QueryString
        /// </summary>
        /// <returns></returns>
        private SearchOptions BuildSearchOptions()
        {
            var searchOptions = new SearchOptions
            {
                Keyword = Request.QueryString[QueryString.Keyword],
                ModuleId = ModuleId,
                TabId = TabId,
                Pagination = true,
                IsEditMode = Request.IsAuthenticated || ModulePermissionController.CanEditModuleContent(new ModuleController().GetModule(ModuleId))
            };

            int skip;
            if (int.TryParse(Request.QueryString[QueryString.Skip], out skip))
                searchOptions.Skip = skip;

            int limit;
            if (int.TryParse(Request.QueryString[QueryString.Limit], out limit) && limit > 0 && limit <= 100)
                searchOptions.Limit = limit;
            else
                searchOptions.Limit = GetLimitFromTabModuleSettings();

            return searchOptions;
        }

        /// <summary>
        /// Get Limit (Paging value) from Tab Module Settings
        /// </summary>
        /// <returns></returns>
        private int GetLimitFromTabModuleSettings()
        {
            var controller = new ModuleController();
            var settings = controller.GetTabModuleSettings(TabModuleId);
            int limit;
            if (settings.ContainsKey(SettingName.Paging) &&
                int.TryParse(settings[SettingName.Paging].ToString(), out limit))
                return limit;
            return 10;
        }

        #endregion
    }
}