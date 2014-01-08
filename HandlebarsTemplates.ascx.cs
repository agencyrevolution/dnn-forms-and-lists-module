using System;
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Controllers;
using DotNetNuke.Modules.UserDefinedTable.Models;
using DotNetNuke.Modules.UserDefinedTable.Models.HandlebarsTemplates;
using DotNetNuke.Modules.UserDefinedTable.Models.SearchOptions;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;
using Rev.RestClients;


namespace DotNetNuke.Modules.UserDefinedTable
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Manage Handlebars Template
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class HandlebarsTemplates : ModuleUserControlBase, IActionable
    {
        #region Constants & Properties

        public enum ErrorOutput
        {
            TokenTemplate,
            XslTranformation,
            Save
        }

        string ReturnUrl
        {
            get { return ViewState["ReturnUrl"].AsString(ModuleContext.EditUrl("Manage")); }
            set { ViewState["ReturnUrl"] = value; }
        }

        #endregion

        #region Private Methods
        
        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            cmdSave.Click += cmdSave_Click;
            cmdBack.Click += cmdBack_Click;
            btnGetJson.Click += btnGetJson_Click;
            btnGetHtml.Click += btnGetHtml_Click;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            BindData();

            if (Request.QueryString["fileid"].AsString().Length <= 0 &&
                Request.QueryString["Edit"].AsString().ToLowerInvariant() != "current") return;
            ReturnUrl = Request.QueryString["Edit"].AsString().ToLowerInvariant() == "current"
                ? Globals.NavigateURL()
                : ModuleContext.EditUrl("Manage");

        }

        /// <summary>
        /// Get JSON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetJson_Click(object sender, EventArgs e)
        {
            var searchOptions = new SearchOptions
            {
                ModuleId = ModuleContext.ModuleId,
                TabId = ModuleContext.TabId,
                Pagination = true
            };

            int skip;
            searchOptions.Skip = int.TryParse(txtSkip.Text, out skip) ? skip : 0;

            int limit;
            searchOptions.Limit = int.TryParse(txtLimit.Text, out limit) ? limit : 10;

            txtJson.Text = new DataController().GetJsonString(searchOptions, Request.Url.LocalPath);
        }

        /// <summary>
        /// Handle the click event on GetHtml button.
        /// Send request to nodejs power template app to get html from Json and Handlebars template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnGetHtml_Click(object sender, EventArgs e)
        {
            var client = new TemplateRestClient();
            var json = JsonHelpers.NormalizeJson(txtJson.Text);
            var html = client.GetHtmlContent(txtTemplateContent.Text, json);
            txtHtml.Text = html;
            panelPreview.Controls.Clear();
            panelPreview.Controls.Add(new LiteralControl(html));
        }

        /// <summary>
        /// Handle the click event on Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(ReturnUrl, true);
        }

        /// <summary>
        /// Handle the click event on Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Validate())
                return;

            SaveHandlebarsTemplate();
            UpdateSettings();
            Response.Redirect(ReturnUrl, true);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Fetch data from database and bind to ASP controls
        /// </summary>
        private void BindData()
        {
            var moduleController = new ModuleController();
            var tabModuleSettings = moduleController.GetTabModuleSettings(ModuleContext.TabModuleId);
            if (!tabModuleSettings.ContainsKey(SettingName.RenderingMethod)) return;

            var renderingMethod = tabModuleSettings[SettingName.RenderingMethod].ToString();
            if (!renderingMethod.Equals(RenderingMethod.UserDefinedHandlebarsTemplate)) return;

            var template = new HandlebarsTemplateController().GetTemplateByModuleId(ModuleContext.ModuleId);
            if (template == null) return;

            txtTemplateContent.Text = template.TemplateString;
            txtRequiredJavaScripts.Text = template.RequiredJavaScripts;
            txtRequiredStylesheets.Text = template.RequiredStylesheets;
        }

        /// <summary>
        /// Validate Handlebars template before saving
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            var client = new TemplateRestClient();
            var html = client.GetHtmlContent(txtTemplateContent.Text, txtJson.Text);
            return !string.IsNullOrEmpty(html);
        }

        /// <summary>
        /// Save Handlebars Template
        /// </summary>
        private void SaveHandlebarsTemplate()
        {
            new HandlebarsTemplateController().SaveTemplate(new HandlebarsTemplate
            {
                ModuleId = ModuleContext.ModuleId,
                RequiredJavaScripts = txtRequiredJavaScripts.Text,
                RequiredStylesheets = txtRequiredStylesheets.Text,
                TemplateString = txtTemplateContent.Text
            });
        }

        /// <summary>
        /// Update Settings
        /// </summary>
        private void UpdateSettings()
        {
            var moduleController = new ModuleController();
            moduleController.UpdateTabModuleSetting(ModuleContext.TabModuleId, SettingName.RenderingMethod,
                RenderingMethod.UserDefinedHandlebarsTemplate);
        }

        #endregion

        #region IActionable

        /// <summary>
        /// List of Module Actions for this view
        /// </summary>
        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();
                actions.Add(ModuleContext.GetNextActionID(),
                    Localization.GetString("ShowJson.Action", LocalResourceFile), string.Empty, string.Empty,
                    string.Empty,
                    ResolveUrl(string.Format("~{0}ShowJson.ashx?{1}={2}&{3}={4}", Definition.PathOfModule,
                        QueryString.TabId, ModuleContext.TabId, QueryString.ModuleId, ModuleContext.ModuleId)), false,
                    SecurityAccessLevel.Edit, true, true);
                return actions;
            }
        }

        #endregion
    }
}