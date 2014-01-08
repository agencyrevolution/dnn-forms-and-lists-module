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
using DotNetNuke.Services.Exceptions;

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

                // template
                var template = new HandlebarsTemplateController().GetTemplateByModuleId(ModuleId);
                if (template == null || string.IsNullOrEmpty(template.TemplateString))
                    throw new Exception("Template content not found");
                searchOptions.TemplateContent = template.TemplateString;

                // required JavaScripts
                RegisterJavaScripts(template.RequiredJavaScripts);

                // required Stylesheets
                RegisterStylesheets(template.RequiredStylesheets);

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

        #endregion

        #region Private Methods

        /// <summary>
        /// Register JavaScripts
        /// </summary>
        /// <param name="requiredJavaScripts"></param>
        private void RegisterJavaScripts(string requiredJavaScripts)
        {
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
        /// <param name="requiredStylesheets"></param>
        private void RegisterStylesheets(string requiredStylesheets)
        {
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
                Pagination = true
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