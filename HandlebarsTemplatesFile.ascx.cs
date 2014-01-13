using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Controllers;
using DotNetNuke.Modules.UserDefinedTable.Models;
using DotNetNuke.Modules.UserDefinedTable.Models.SearchOptions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
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
    public partial class HandlebarsTemplatesFile : ModuleUserControlBase, IActionable
    {
        #region Constants & Properties

        private const string DefaultExtension = ".txt";

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

        /// <summary>
        /// Get File Id
        /// </summary>
        public int FileId
        {
            get { return Request.QueryString["FileID"].AsInt(); }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// OnInit event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            cmdSave.Click += cmdSave_Click;
            cmdBack.Click += cmdBack_Click;
            btnGetJson.Click += btnGetJson_Click;
            btnGetHtml.Click += btnGetHtml_Click;
        }

        /// <summary>
        /// Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            if (FileId > 0 || Request.QueryString[QueryString.Edit].AsString().ToLowerInvariant() == "current")
            {
                IFileInfo file = null;
                if (Request.QueryString[QueryString.Edit].AsString().ToLowerInvariant() == "current")
                {
                    var relativePath = ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateUrl].ToString();
                    if (!string.IsNullOrEmpty(relativePath))
                        file = FileManager.Instance.GetFile(ModuleContext.PortalId, relativePath);
                    ReturnUrl = Globals.NavigateURL();
                }
                else
                {
                    var fileId = int.Parse(Request.QueryString[QueryString.FileId]);
                    file = FileManager.Instance.GetFile(fileId);
                    ReturnUrl = ModuleContext.EditUrl(ControlKeys.Manage);
                }

                if (file != null && (HasWritePermission(file.Folder)))
                {
                    EditExistingTemplate(file);
                }
                else
                {
                    InitNewTemplate();
                }
            }
            else
            {
                InitNewTemplate();
            }

            // required JavaScripts
            if (ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts))
                txtRequiredJavaScripts.Text =
                    ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts].AsString();

            // required Stylesheets
            if (ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets))
                txtRequiredStylesheets.Text =
                    ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets].AsString();

            ReturnUrl = Request.QueryString[QueryString.Edit].AsString().ToLowerInvariant() == "current"
                ? Globals.NavigateURL()
                : ModuleContext.EditUrl(ControlKeys.Manage);
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

            var folder = GetFolder();
            if (folder == null)
            {
                ShowError("You must select a Folder");
                return;
            }

            var file = SaveHandlebarsTemplate(txtTemplateContent.Text, GetFileName(), folder, true);
            if (file == null)
            {
                ShowError("Sorry... could not save Handlebars template to the chosen File!");
                return;
            }

            // save settings and redirect
            UpdateSettings(file);
            Response.Redirect(ReturnUrl, true);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Check if current user has write permission to the folder or not
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        bool HasWritePermission(string folderPath)
        {
            var folderInfo = FolderManager.Instance.GetFolder(ModuleContext.PortalId, folderPath);
            return (folderInfo is FolderInfo && FolderPermissionController.CanAdminFolder((FolderInfo)folderInfo));
        }

        /// <summary>
        /// Show error message to let user know
        /// </summary>
        /// <param name="message"></param>
        private void ShowError(string message)
        {
            panelError.Visible = !string.IsNullOrEmpty(message);
            if(!string.IsNullOrEmpty(message))
                panelError.Controls.Add(new LiteralControl(message));
        }

        /// <summary>
        /// Edit exsting template
        /// </summary>
        /// <param name="file"></param>
        private void EditExistingTemplate(IFileInfo file)
        {
            var folder = FolderManager.Instance.GetFolder(file.FolderId);
            if (folder == null)
            {
                ShowError("Invalid File");
                return;
            }

            // bind data for Folders drop down list
            ddlFolders.Items.Add(new ListItem(folder.FolderName, folder.FolderID.ToString(CultureInfo.InvariantCulture)));
            ddlFolders.Enabled = false;
            
            txtFileName.Enabled = false;
            txtFileName.Text = file.FileName;

            // content template
            txtTemplateContent.Text = ReadTemplateContentFromFile(file.FileId);
        }

        /// <summary>
        /// Initialize new template
        /// </summary>
        private void InitNewTemplate()
        {
            // bind data for Folders drop down list
            var folders = FolderManager.Instance.GetFolders(UserController.GetCurrentUserInfo());
            var folderInfos = folders as IList<IFolderInfo> ?? folders.ToList();
            BindDataForFoldersDropDownList(folderInfos, string.Empty);
            
            // file name
            txtFileName.Text = string.Format("FormsAndLists_Module{0}.txt", ModuleContext.ModuleId);
        }

        /// <summary>
        /// Bind data for Folders drop down list
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="selectedValue"></param>
        void BindDataForFoldersDropDownList(IEnumerable<IFolderInfo> folders, string selectedValue)
        {
            ddlFolders.DataSource = folders;
            ddlFolders.DataTextField = "FolderName";
            ddlFolders.DataValueField = "FolderID";
            ddlFolders.DataBind();
            ddlFolders.SelectedValue = selectedValue;
        }

        /// <summary>
        /// Normalize file name with .hd extension
        /// </summary>
        /// <returns></returns>
        string GetFileName()
        {
            var fileName = txtFileName.Text.Trim();
            fileName = Globals.CleanFileName(fileName);
            if (!fileName.ToLowerInvariant().EndsWith(DefaultExtension))
            {
                fileName += DefaultExtension;
            }
            txtFileName.Text = fileName;
            return fileName;
        }

        /// <summary>
        /// Update an existing or add a new file
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="fileName"></param>
        /// <param name="folder"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        static IFileInfo SaveHandlebarsTemplate(string fileContent, string fileName, IFolderInfo folder, bool overwrite)
        {
            var utf8 = new UTF8Encoding();
            return FileManager.Instance.AddFile(folder, fileName, new MemoryStream(utf8.GetBytes(fileContent)),
                overwrite);
        }

        /// <summary>
        /// Read template content from file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static string ReadTemplateContentFromFile(int fileId)
        {
            try
            {
                var fileInfo = FileManager.Instance.GetFile(fileId);
                using (var stream = FileManager.Instance.GetFileContent(fileInfo))
                {
                    return new StreamReader(stream, Encoding.ASCII).ReadToEnd();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Folder to store template
        /// </summary>
        /// <returns></returns>
        private IFolderInfo GetFolder()
        {
            int folderId;
            return !int.TryParse(ddlFolders.SelectedValue, out folderId)
                ? null
                : FolderManager.Instance.GetFolder(folderId);
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
        /// Update Tab Module Setting
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="settingValue"></param>
        private void UpdateTabModuleSetting(string settingKey, string settingValue)
        {
            new ModuleController().UpdateTabModuleSetting(ModuleContext.TabModuleId, settingKey, settingValue);
            ModuleContext.Settings[settingKey] = settingValue;
        }

        /// <summary>
        /// Update Settings
        /// </summary>
        private void UpdateSettings(IFileInfo file)
        {
            UpdateTabModuleSetting(SettingName.RenderingMethod, RenderingMethod.UserDefinedHandlebarsTemplate);
            UpdateTabModuleSetting(SettingName.UserDefinedHandlebarsTemplateUrl, file.Folder + file.FileName);
            UpdateTabModuleSetting(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts,
                txtRequiredJavaScripts.Text);
            UpdateTabModuleSetting(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets,
                txtRequiredStylesheets.Text);
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
                var actions = new ModuleActionCollection
                {
                    {
                        ModuleContext.GetNextActionID(), Localization.GetString("ShowJson.Action", LocalResourceFile),
                        string.Empty, string.Empty, string.Empty,
                        ResolveUrl(string.Format("~{0}ShowJson.ashx?{1}={2}&{3}={4}", Definition.PathOfModule,
                            QueryString.TabId, ModuleContext.TabId, QueryString.ModuleId, ModuleContext.ModuleId)),
                        false, SecurityAccessLevel.Edit, true, true
                    }
                };
                return actions;
            }
        }

        #endregion
    }
}