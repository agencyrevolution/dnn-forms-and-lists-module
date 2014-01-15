using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Models.Settings;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;

namespace DotNetNuke.Modules.UserDefinedTable
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     Manage Handlebars Template
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ImportModuleSettings : ModuleUserControlBase
    {
        #region Constants & Properties

        private const string ImportSettingsKey = "FNL_ImportSettings";

        private ModuleController _objModules;
        public ModuleController ModulesController
        {
            get { return _objModules ?? (_objModules = new ModuleController()); }
        }

        private string ReturnUrl
        {
            get { return ViewState["ReturnUrl"].AsString(ModuleContext.EditUrl(ControlKeys.Manage)); }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///     OnInit event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            btnUpload.Click += btnUpload_Click;
            cmdImport.Click += cmdImport_Click;
            cmdBack.Click += cmdBack_Click;
        }

        /// <summary>
        ///     Upload button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileImport.FileName))
            {
                ShowError("You need to choose a File from your local then click on Upload");
                return;
            }

            try
            {
                var memoryStream = new MemoryStream();
                fileImport.FileContent.CopyTo(memoryStream);
                string content = Encoding.UTF8.GetString(memoryStream.ToArray());

                var importSettings = JsonConvert.DeserializeObject<ImportSettings>(content);
                if (importSettings == null)
                {
                    ShowError(
                        "Cannot parse the imported file. It would be in an incorrect JSON format. Please retry to export settings from source module.");
                    return;
                }

                ViewState[ImportSettingsKey] = importSettings;

                // show preview
                ShowPreview(importSettings);
            }
            catch (Exception)
            {
                ShowError(
                    "Cannot parse the imported file. It would be in an incorrect JSON format. Please retry to export settings from source module.");
            }
        }

        /// <summary>
        ///     Handle the click event on Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(ReturnUrl, true);
        }

        /// <summary>
        ///     Handle the click event on Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdImport_Click(object sender, EventArgs e)
        {
            var settings = ViewState[ImportSettingsKey] as ImportSettings;
            if (settings == null)
            {
                ShowError(
                    "Your session has been expired. Please upload the exported settings file again then try to import!");
                return;
            }

            // save settings to current module
            SaveSettings(settings);

            // redirect Back
            if (!panelError.Visible)
                Response.Redirect(ReturnUrl, true);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        ///     Show preview and let user to choose which settings he want to force overriding
        /// </summary>
        /// <param name="importSettings"></param>
        private void ShowPreview(ImportSettings importSettings)
        {
            panelPreview.Visible = true;
            cmdImport.Visible = true;
            panelError.Visible = false;

            BindData(importSettings);
        }

        /// <summary>
        ///     Show error message to let user know
        /// </summary>
        /// <param name="message"></param>
        private void ShowError(string message)
        {
            panelError.Visible = !string.IsNullOrEmpty(message);
            if (!string.IsNullOrEmpty(message))
                panelError.Controls.Add(new LiteralControl(message));
        }

        /// <summary>
        ///     Bind data for all data
        /// </summary>
        private void BindData(ImportSettings settings)
        {
            // module settings
            BindData(SettingName.EditOnlyOwnItems, settings.ModuleSettings.EditOnlyOwnItems, chkEditOwnData,
                phEditOwnData, phEditOwnDataNew);
            BindData(SettingName.ForceCaptchaForAnonymous, settings.ModuleSettings.ForceCaptchaForAnonymous,
                chkForceCaptcha, phForceCaptcha, phForceCaptchaNew);
            BindData(SettingName.ForceInputFiltering, settings.ModuleSettings.ForceInputFiltering, chkInputFiltering,
                phInputFiltering, phInputFilteringNew);
            BindData(SettingName.ShowAllColumnsForAdmins, settings.ModuleSettings.ShowAllColumnsForAdmins,
                chkDisplayColumns, phDisplayColumns, phDisplayColumnsNew);
            BindData(SettingName.ShowSystemColumns, settings.ModuleSettings.ShowSystemColumns, chkHideSystemColumns,
                phHideSystemColumns, phHideSystemColumnsNew);
            BindData(SettingName.EditPrivateColumnsForAdmins, settings.ModuleSettings.EditPrivateColumnsForAdmins,
                chkPrivateColumns, phPrivateColumns, phPrivateColumnsNew);
            BindData(SettingName.UserRecordQuota, settings.ModuleSettings.UserRecordQuota, chkUserRecordQuota,
                phUserRecordQuota, phUserRecordQuotaNew);

            // current view options
            BindData(SettingName.ListOrForm, settings.ModuleConfiguration.CurrentViewOptions.Appearance, chkAppearance,
                phAppearance, phAppearanceNew);
            BindData(SettingName.UseButtonsInForm, settings.ModuleConfiguration.CurrentViewOptions.UseButtonsInForm,
                chkUseButtonsInForm, phUseButtonsInForm, phUseButtonsInFormNew);

            // form settings
            BindData(SettingName.UponSubmitAction, settings.ModuleConfiguration.FormSettings.UponSubmitAction,
                chkUponSubmitAction, phUponSubmitAction, phUponSubmitActionNew);
            BindData(SettingName.UponSubmitRedirect, settings.ModuleConfiguration.FormSettings.UponSubmitRedirect,
                chkUponSubmitRedirect, phUponSubmitRedirect, phUponSubmitRedirectNew);
            BindData(SettingName.SubmissionText, settings.ModuleConfiguration.FormSettings.SubmissionText,
                chkSubmissionText, phSubmissionText, phSubmissionTextNew);

            // list settings
            BindData(SettingName.RenderingMethod, settings.ModuleConfiguration.ListSettings.RenderingMethod,
                chkRenderingMethod, phRenderingMethod, phRenderingMethodNew);

            // search options
            BindData(SettingName.ExcludeFromSearch,
                settings.ModuleConfiguration.ListSettings.SearchSettings.ExcludeFromSearch, chkExcludeFromSearch,
                phExcludeFromSearch, phExcludeFromSearchNew);
            BindData(SettingName.SortOrder, settings.ModuleConfiguration.ListSettings.SearchSettings.SortOrder,
                chkSortOrder, phSortOrder, phSortOrderNew);
            BindData(SettingName.SortField, settings.ModuleConfiguration.ListSettings.SearchSettings.SortField,
                chkSortField, phSortField, phSortFieldNew);
            BindData(SettingName.Paging, settings.ModuleConfiguration.ListSettings.SearchSettings.Paging, chkPaging,
                phPaging, phPagingNew);
            BindData(SettingName.Filter, settings.ModuleConfiguration.ListSettings.SearchSettings.Filter, chkFilter,
                phFilter, phFilterNew);
            BindData(SettingName.TopCount, settings.ModuleConfiguration.ListSettings.SearchSettings.TopCount,
                chkTopCount, phTopCount, phTopCountNew);
            BindData(SettingName.ShowSearchTextBox,
                settings.ModuleConfiguration.ListSettings.SearchSettings.ShowSearchTextBox, chkShowSearchTextBox,
                phShowSearchTextBox, phShowSearchTextBoxNew);
            BindData(SettingName.ShowNoRecordsUntilSearch,
                settings.ModuleConfiguration.ListSettings.SearchSettings.ShowNoRecordsUntilSearch,
                chkShowNoRecordsUntilSearch, phShowNoRecordsUntilSearch, phShowNoRecordsUntilSearchNew);
            BindData(SettingName.SimpleSearch, settings.ModuleConfiguration.ListSettings.SearchSettings.SimpleSearch,
                chkSimpleSearch, phSimpleSearch, phSimpleSearchNew);
            BindData(SettingName.URLSearch, settings.ModuleConfiguration.ListSettings.SearchSettings.UrlSearch,
                chkUrlSearch, phUrlSearch, phUrlSearchNew);

            // xsl template settings
            BindData(SettingName.XslUserDefinedStyleSheet,
                settings.ModuleConfiguration.ListSettings.XslTemplateSettings.TemplateUrl, chkXslTemplateUrl,
                phXslTemplateUrl, phXslTemplateUrlNew);
            BindDataForTemplateContent(SettingName.XslUserDefinedStyleSheet, chkXslTemplateContent, phXslTemplateContent,
                settings.ModuleConfiguration.ListSettings.XslTemplateSettings.TemplateContent, phXslTemplateContentNew);

            // handlebars template settings
            BindData(SettingName.UserDefinedHandlebarsTemplateUrl,
                settings.ModuleConfiguration.ListSettings.HandlebarsTemplateSettings.TemplateUrl,
                chkHandlebarsTemplateUrl, phHandlebarsTemplateUrl, phHandlebarsTemplateUrlNew);
            BindDataForTemplateContent(SettingName.UserDefinedHandlebarsTemplateUrl, chkHandlebarsTemplateContent,
                phHandlebarsTemplateContent,
                settings.ModuleConfiguration.ListSettings.HandlebarsTemplateSettings.TemplateContent,
                phHandlebarsTemplateContentNew);
            BindData(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts,
                settings.ModuleConfiguration.ListSettings.HandlebarsTemplateSettings.RequiredJavaScripts,
                chkRequiredJavaScripts, phRequiredJavaScripts, phRequiredJavaScriptsNew);
            BindData(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets,
                settings.ModuleConfiguration.ListSettings.HandlebarsTemplateSettings.RequiredStylesheets,
                chkRequiredStylesheets, phRequiredStylesheets, phRequiredStylesheetsNew);

            // email settings
            BindData(SettingName.TrackingScript, settings.ModuleConfiguration.EmailSettings.TrackingScripts,
                chkTrackingScripts, phTrackingScripts, phTrackingScriptsNew);
            BindData(SettingName.TrackingEmailTo, settings.ModuleConfiguration.EmailSettings.TrackingEmailTo,
                chkTrackingEmailTo, phTrackingEmailTo, phTrackingEmailToNew);
            BindData(SettingName.TrackingEmailCc, settings.ModuleConfiguration.EmailSettings.TrackingEmailCc,
                chkTrackingEmailCc, phTrackingEmailCc, phTrackingEmailCcNew);
            BindData(SettingName.TrackingEmailBcc, settings.ModuleConfiguration.EmailSettings.TrackingEmailBcc,
                chkTrackingEmailBcc, phTrackingEmailBcc, phTrackingEmailBccNew);
            BindData(SettingName.TrackingEmailReplyTo, settings.ModuleConfiguration.EmailSettings.TrackingEmailReplyTo,
                chkTrackingEmailReplyTo, phTrackingEmailReplyTo, phTrackingEmailReplyToNew);
            BindData(SettingName.TrackingEmailFrom, settings.ModuleConfiguration.EmailSettings.TrackingEmailFrom,
                chkTrackingEmailFrom, phTrackingEmailFrom, phTrackingEmailFromNew);
            BindData(SettingName.TrackingSubject, settings.ModuleConfiguration.EmailSettings.TrackingSubject,
                chkTrackingSubject, phTrackingSubject, phTrackingSubjectNew);
            BindData(SettingName.TrackingMessage, settings.ModuleConfiguration.EmailSettings.TrackingMessage,
                chkTrackingMessage, phTrackingMessage, phTrackingMessageNew);
            BindData(SettingName.TrackingTriggerOnNew, settings.ModuleConfiguration.EmailSettings.TrackingTriggerOnNew,
                chkTrackingTriggerOnNew, phTrackingTriggerOnNew, phTrackingTriggerOnNewNew);
            BindData(SettingName.TrackingTriggerOnUpdate,
                settings.ModuleConfiguration.EmailSettings.TrackingTriggerOnUpdate, chkTrackingTriggerOnUpdate,
                phTrackingTriggerOnUpdate, phTrackingTriggerOnUpdateNew);
            BindData(SettingName.TrackingTriggerOnDelete,
                settings.ModuleConfiguration.EmailSettings.TrackingTriggerOnDelete, chkTrackingTriggerOnDelete,
                phTrackingTriggerOnDelete, phTrackingTriggerOnDeleteNew);
            BindData(SettingName.TrackingTextOnNew, settings.ModuleConfiguration.EmailSettings.TrackingTextOnNew,
                chkTrackingTextOnNew, phTrackingTextOnNew, phTrackingTextOnNewNew);
            BindData(SettingName.TrackingTextOnUpdate, settings.ModuleConfiguration.EmailSettings.TrackingTextOnUpdate,
                chkTrackingTextOnUpdate, phTrackingTextOnUpdate, phTrackingTextOnUpdateNew);
            BindData(SettingName.TrackingTextOnDelete, settings.ModuleConfiguration.EmailSettings.TrackingTextOnDelete,
                chkTrackingTextOnDelete, phTrackingTextOnDelete, phTrackingTextOnDeleteNew);
        }

        /// <summary>
        ///     Bind data for a setting name
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="newSettingValue"></param>
        /// <param name="checkbox"></param>
        /// <param name="placeHolderCurretnSettingValue"></param>
        /// <param name="placeHolderNewSettingValue"></param>
        private void BindData(string settingName, string newSettingValue, CheckBox checkbox,
            PlaceHolder placeHolderCurretnSettingValue, PlaceHolder placeHolderNewSettingValue)
        {
            if (!string.IsNullOrEmpty(newSettingValue))
                placeHolderNewSettingValue.Controls.Add(new LiteralControl(newSettingValue));

            string currentSettingValue = GetSettingValue(settingName);
            if (!string.IsNullOrEmpty(currentSettingValue))
                placeHolderCurretnSettingValue.Controls.Add(new LiteralControl(currentSettingValue));

            checkbox.Checked = string.IsNullOrEmpty(currentSettingValue) && !string.IsNullOrEmpty(newSettingValue);
        }


        /// <summary>
        ///     Read template content from file, then bind to proper placeholder
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="checkBox"></param>
        /// <param name="placeHolderCurrentSettingValue"></param>
        /// <param name="newSettingValue"></param>
        /// <param name="placeHolderNewSettingValue"></param>
        private void BindDataForTemplateContent(string settingName, CheckBox checkBox,
            PlaceHolder placeHolderCurrentSettingValue, string newSettingValue, PlaceHolder placeHolderNewSettingValue)
        {
            string relativePath = GetSettingValue(settingName);
            string currentTemplateContent = ReadTemplateContentFromFile(relativePath);
            if (!string.IsNullOrEmpty(currentTemplateContent))
                placeHolderCurrentSettingValue.Controls.Add(
                    new LiteralControl(HttpUtility.HtmlEncode(currentTemplateContent)));

            if (!string.IsNullOrEmpty(newSettingValue))
                placeHolderNewSettingValue.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(newSettingValue)));

            checkBox.Checked = string.IsNullOrEmpty(currentTemplateContent) && !string.IsNullOrEmpty(newSettingValue);
        }

        /// <summary>
        ///     Get setting value from ModuleContext.Settings
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        private string GetSettingValue(string settingName)
        {
            return ModuleContext.Settings.ContainsKey(settingName)
                ? ModuleContext.Settings[settingName].AsString()
                : null;
        }

        /// <summary>
        ///     Read template content from file
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private string ReadTemplateContentFromFile(string relativePath)
        {
            try
            {
                var fileInfo = FileManager.Instance.GetFile(ModuleContext.PortalId, relativePath);
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
        ///     Show error when saving file to DNN portal
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <param name="ex"></param>
        private void ShowError(string folderPath, string filename, string content, Exception ex = null)
        {
            ShowError(
                "Cannot import template content to module settings because you do not have permission to save file to DNN portal");
            ShowError(string.Format("folderPath: {0}", folderPath));
            ShowError(string.Format("fileName: {0}", filename));
            ShowError(string.Format("content: {0}", HttpUtility.HtmlEncode(content)));

            if (ex != null)
                ShowError(string.Format("Error Message: {0}", ex.Message));
        }

        /// <summary>
        /// Save Template content to Dnn file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="relativePath"></param>
        private void SaveTemplateContentToFile(string content, string relativePath)
        {
            string folderPath;
            string fileName;
            if (TryParseFileInfo(relativePath, out folderPath, out fileName))
                SaveTemplateContentToFile(folderPath, fileName, content);
        }

        /// <summary>
        /// Parse folderPath and fileName from relativePath
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        private bool TryParseFileInfo(string relativePath, out string folderPath, out string fileName)
        {
            folderPath = string.Empty;
            fileName = string.Empty;

            if (string.IsNullOrEmpty(relativePath))
                return false;

            var index = relativePath.LastIndexOf('/');
            if (index < 0)
            {
                folderPath = string.Empty;
                fileName = relativePath;
            }
            else if (index < relativePath.Length - 1)
            {
                folderPath = relativePath.Substring(0, index - 1);
                fileName = relativePath.Substring(index + 1);
            }
            else
            {
                ShowError("Invalid DNN relative path");
                ShowError(string.Format("relativePath: {0}", relativePath));
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Save template content to file
        /// </summary>
        private void SaveTemplateContentToFile(string folderPath, string filename, string content)
        {
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(content))
                return;

            var folder = EnsureFolder(folderPath);
            if (folder == null)
            {
                ShowError(folderPath, filename, content);
                return;
            }

            try
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                var memoryStream = new MemoryStream(bytes);
                var file = FileManager.Instance.AddFile(folder, filename, memoryStream, true);
                if (file == null)
                    ShowError(folderPath, filename, content);
            }
            catch (Exception ex)
            {
                ShowError(folderPath, filename, content, ex);
            }
        }

        /// <summary>
        ///     Ensure folder be created if not existed
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private IFolderInfo EnsureFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return null;

            try
            {
                return !FolderManager.Instance.FolderExists(ModuleContext.PortalId, folderPath)
                    ? FolderManager.Instance.AddFolder(ModuleContext.PortalId, folderPath)
                    : FolderManager.Instance.GetFolder(ModuleContext.PortalId, folderPath);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Folder path {0} not found. Error: {1}", folderPath, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Save setting
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="settingValue"></param>
        private void SaveSetting(string settingKey, string settingValue)
        {
            ModulesController.UpdateTabModuleSetting(ModuleContext.TabModuleId, settingKey, settingValue);
            ModuleContext.Settings[settingKey] = settingValue;
        }

        /// <summary>
        ///     Save settings
        /// </summary>
        /// <returns></returns>
        private void SaveSettings(SettingsBase settings)
        {
            SaveModuleSettings(settings.ModuleSettings);
            SaveModuleConfiguration(settings.ModuleConfiguration);
        }

        /// <summary>
        ///     Build Module Settings
        /// </summary>
        /// <returns></returns>
        private void SaveModuleSettings(ModuleSettings settings)
        {
            if (chkEditOwnData.Checked)
                SaveSetting(SettingName.EditOnlyOwnItems, settings.EditOnlyOwnItems);

            if (chkPrivateColumns.Checked)
                SaveSetting(SettingName.EditPrivateColumnsForAdmins, settings.EditPrivateColumnsForAdmins);

            if (chkForceCaptcha.Checked)
                SaveSetting(SettingName.ForceCaptchaForAnonymous, settings.ForceCaptchaForAnonymous);

            if (chkDisplayColumns.Checked)
                SaveSetting(SettingName.ShowAllColumnsForAdmins, settings.ShowAllColumnsForAdmins);

            if (chkHideSystemColumns.Checked)
                SaveSetting(SettingName.ShowSystemColumns, settings.ShowSystemColumns);

            if (chkUserRecordQuota.Checked)
                SaveSetting(SettingName.UserRecordQuota, settings.UserRecordQuota);

            if (chkInputFiltering.Checked)
                SaveSetting(SettingName.ForceInputFiltering, settings.ForceInputFiltering);
        }

        /// <summary>
        ///     Save Module Configuration
        /// </summary>
        /// <returns></returns>
        private void SaveModuleConfiguration(ModuleConfiguration settings)
        {
            SaveCurrentViewOptions(settings.CurrentViewOptions);
            SaveEmailSettings(settings.EmailSettings);
            SaveFormSettings(settings.FormSettings);
            SaveListSettings(settings.ListSettings);
        }

        /// <summary>
        ///     Save CurrentViewOptions
        /// </summary>
        /// <returns></returns>
        private void SaveCurrentViewOptions(CurrentViewOptions settings)
        {
            if (chkAppearance.Checked)
                SaveSetting(SettingName.ListOrForm, settings.Appearance);

            if (chkUseButtonsInForm.Checked)
                SaveSetting(SettingName.UseButtonsInForm, settings.UseButtonsInForm);
        }

        /// <summary>
        ///     Save Email Settings
        /// </summary>
        /// <returns></returns>
        private void SaveEmailSettings(EmailSettings settings)
        {
            if (chkTrackingEmailBcc.Checked)
                SaveSetting(SettingName.TrackingEmailBcc, settings.TrackingEmailBcc);

            if (chkTrackingEmailCc.Checked)
                SaveSetting(SettingName.TrackingEmailCc, settings.TrackingEmailCc);

            if (chkTrackingEmailFrom.Checked)
                SaveSetting(SettingName.TrackingEmailFrom, settings.TrackingEmailFrom);

            if (chkTrackingEmailReplyTo.Checked)
                SaveSetting(SettingName.TrackingEmailReplyTo, settings.TrackingEmailReplyTo);

            if (chkTrackingEmailTo.Checked)
                SaveSetting(SettingName.TrackingEmailTo, settings.TrackingEmailTo);

            if (chkTrackingMessage.Checked)
                SaveSetting(SettingName.TrackingMessage, settings.TrackingMessage);

            if (chkTrackingScripts.Checked)
                SaveSetting(SettingName.TrackingScript, settings.TrackingScripts);

            if (chkTrackingSubject.Checked)
                SaveSetting(SettingName.TrackingSubject, settings.TrackingSubject);

            if (chkTrackingTextOnDelete.Checked)
                SaveSetting(SettingName.TrackingTextOnDelete, settings.TrackingTextOnDelete);

            if (chkTrackingTextOnNew.Checked)
                SaveSetting(SettingName.TrackingTextOnNew, settings.TrackingTextOnNew);

            if (chkTrackingTextOnUpdate.Checked)
                SaveSetting(SettingName.TrackingTextOnUpdate, settings.TrackingTextOnUpdate);

            if (chkTrackingTriggerOnDelete.Checked)
                SaveSetting(SettingName.TrackingTriggerOnDelete, settings.TrackingTriggerOnDelete);

            if (chkTrackingTriggerOnNew.Checked)
                SaveSetting(SettingName.TrackingTriggerOnNew, settings.TrackingTriggerOnNew);

            if (chkTrackingTriggerOnUpdate.Checked)
                SaveSetting(SettingName.TrackingTriggerOnUpdate, settings.TrackingTriggerOnUpdate);
        }

        /// <summary>
        ///     Save forms settings
        /// </summary>
        /// <returns></returns>
        private void SaveFormSettings(FormSettings settings)
        {
            if (chkSubmissionText.Checked)
                SaveSetting(SettingName.SubmissionText, settings.SubmissionText);

            if (chkUponSubmitAction.Checked)
                SaveSetting(SettingName.UponSubmitAction, settings.UponSubmitAction);

            if (chkUponSubmitRedirect.Checked)
                SaveSetting(SettingName.UponSubmitRedirect, settings.UponSubmitRedirect);
        }

        /// <summary>
        ///     Save list settings
        /// </summary>
        /// <returns></returns>
        private void SaveListSettings(ListSettings settings)
        {
            if (chkRenderingMethod.Checked)
                SaveSetting(SettingName.RenderingMethod, settings.RenderingMethod);

            SaveSearchSettings(settings.SearchSettings);
            SaveXslTemplateSettings(settings.XslTemplateSettings);
            SaveHandlebarsTemplateSettings(settings.HandlebarsTemplateSettings);
        }

        /// <summary>
        ///     Save search settings
        /// </summary>
        /// <returns></returns>
        private void SaveSearchSettings(SearchSettings settings)
        {
            if (chkExcludeFromSearch.Checked)
                SaveSetting(SettingName.ExcludeFromSearch, settings.ExcludeFromSearch);

            if (chkFilter.Checked)
                SaveSetting(SettingName.Filter, settings.Filter);

            if (chkPaging.Checked)
                SaveSetting(SettingName.Paging, settings.Paging);

            if (chkShowNoRecordsUntilSearch.Checked)
                SaveSetting(SettingName.ShowNoRecordsUntilSearch, settings.ShowNoRecordsUntilSearch);

            if (chkShowSearchTextBox.Checked)
                SaveSetting(SettingName.ShowSearchTextBox, settings.ShowSearchTextBox);

            if (chkSimpleSearch.Checked)
                SaveSetting(SettingName.SimpleSearch, settings.SimpleSearch);

            if (chkSortField.Checked)
                SaveSetting(SettingName.SortField, settings.SortField);

            if (chkSortOrder.Checked)
                SaveSetting(SettingName.SortOrder, settings.SortOrder);

            if (chkTopCount.Checked)
                SaveSetting(SettingName.TopCount, settings.TopCount);

            if (chkUrlSearch.Checked)
                SaveSetting(SettingName.URLSearch, settings.UrlSearch);
        }

        /// <summary>
        ///     Save xsl template settings
        /// </summary>
        /// <returns></returns>
        private void SaveXslTemplateSettings(XslTemplateSettings settings)
        {
            var xslPath = GetSettingValue(SettingName.XslUserDefinedStyleSheet);
            string folderPath;
            string fileName;
            if (chkXslTemplateUrl.Checked && TryParseFileInfo(settings.TemplateUrl, out folderPath, out fileName))
            {
                SaveSetting(SettingName.XslUserDefinedStyleSheet, settings.TemplateUrl);
                xslPath = settings.TemplateUrl;
            }

            if (chkXslTemplateContent.Checked &&
                !string.IsNullOrEmpty(xslPath) &&
                !string.IsNullOrEmpty(settings.TemplateContent))
            {
                SaveTemplateContentToFile(settings.TemplateContent, xslPath);
            }
        }

        /// <summary>
        /// Build handlebars template settings
        /// </summary>
        /// <returns></returns>
        private void SaveHandlebarsTemplateSettings(HandlebarsTemplateSettings settings)
        {
            if (chkRequiredJavaScripts.Checked)
                SaveSetting(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts, settings.RequiredJavaScripts);

            if (chkRequiredStylesheets.Checked)
                SaveSetting(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets, settings.RequiredStylesheets);

            var templateUrl = GetSettingValue(SettingName.UserDefinedHandlebarsTemplateUrl);
            string folderPath;
            string fileName;
            if (chkHandlebarsTemplateUrl.Checked && TryParseFileInfo(settings.TemplateUrl, out folderPath, out fileName))
            {
                SaveSetting(SettingName.UserDefinedHandlebarsTemplateUrl, settings.TemplateUrl);
                templateUrl = settings.TemplateUrl;
            }

            if (chkHandlebarsTemplateContent.Checked &&
                !string.IsNullOrEmpty(settings.TemplateContent) &&
                !string.IsNullOrEmpty(templateUrl))
                SaveTemplateContentToFile(settings.TemplateContent, templateUrl);
        }

        #endregion
    }
}