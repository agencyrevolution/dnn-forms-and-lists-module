using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Models.Settings;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;


namespace DotNetNuke.Modules.UserDefinedTable
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Manage Handlebars Template
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ExportModuleSettings : ModuleUserControlBase
    {
        #region Constants & Properties

        string ReturnUrl
        {
            get { return ViewState["ReturnUrl"].AsString(ModuleContext.EditUrl("Manage")); }
            set { ViewState["ReturnUrl"] = value; }
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
            cmdExport.Click += cmdExport_Click;
            cmdBack.Click += cmdBack_Click;
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
            
            BindData();
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
        void cmdExport_Click(object sender, EventArgs e)
        {
            var settings = BuildExportSettings();
            var json = JsonConvert.SerializeObject(settings);

            Response.ContentType = "application/json";
            Response.AddHeader("content-disposition", "attachment;filename=moduleSettings.json");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(json);
            Response.End();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Bind data for all data
        /// </summary>
        void BindData()
        {
            // module settings
            BindData(SettingName.EditOnlyOwnItems, chkEditOwnData, phEditOwnData);
            BindData(SettingName.ForceCaptchaForAnonymous, chkForceCaptcha, phForceCaptcha);
            BindData(SettingName.ForceInputFiltering, chkInputFiltering, phInputFiltering);
            BindData(SettingName.ShowAllColumnsForAdmins, chkDisplayColumns, phDisplayColumns);
            BindData(SettingName.ShowSystemColumns, chkHideSystemColumns, phHideSystemColumns);
            BindData(SettingName.EditPrivateColumnsForAdmins, chkPrivateColumns, phPrivateColumns);
            BindData(SettingName.UserRecordQuota, chkUserRecordQuota, phUserRecordQuota);

            // current view options
            BindData(SettingName.ListOrForm, chkAppearance, phAppearance);
            BindData(SettingName.UseButtonsInForm, chkUseButtonsInForm, phUseButtonsInForm);

            // form settings
            BindData(SettingName.UponSubmitAction, chkUponSubmitAction, phUponSubmitAction);
            BindData(SettingName.UponSubmitRedirect, chkUponSubmitRedirect, phUponSubmitRedirect);
            BindData(SettingName.SubmissionText, chkSubmissionText, phSubmissionText);

            // list settings
            BindData(SettingName.RenderingMethod, chkRenderingMethod, phRenderingMethod);

            // search options
            BindData(SettingName.ExcludeFromSearch, chkExcludeFromSearch, phExcludeFromSearch);
            BindData(SettingName.SortOrder, chkSortOrder, phSortOrder);
            BindData(SettingName.SortField, chkSortField, phSortField);
            BindData(SettingName.Paging, chkPaging, phPaging);
            BindData(SettingName.Filter, chkFilter, phFilter);
            BindData(SettingName.TopCount, chkTopCount, phTopCount);
            BindData(SettingName.ShowSearchTextBox, chkShowSearchTextBox, phShowSearchTextBox);
            BindData(SettingName.ShowNoRecordsUntilSearch, chkShowNoRecordsUntilSearch, phShowNoRecordsUntilSearch);
            BindData(SettingName.SimpleSearch, chkSimpleSearch, phSimpleSearch);
            BindData(SettingName.URLSearch, chkUrlSearch, phUrlSearch);

            // xsl template settings
            BindData(SettingName.XslUserDefinedStyleSheet, chkXslTemplateUrl, phXslTemplateUrl);
            BindDataForTemplateContent(SettingName.XslUserDefinedStyleSheet, chkXslTemplateContent, phXslTemplateContent);

            // handlebars template settings
            BindData(SettingName.UserDefinedHandlebarsTemplateUrl, chkHandlebarsTemplateUrl, phHandlebarsTemplateUrl);
            BindDataForTemplateContent(SettingName.UserDefinedHandlebarsTemplateUrl, chkHandlebarsTemplateContent, phHandlebarsTemplateContent);
            BindData(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts, chkRequiredJavaScripts, phRequiredJavaScripts);
            BindData(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets, chkRequiredStylesheets, phRequiredStylesheets);

            // email settings
            BindData(SettingName.TrackingScript, chkTrackingScripts, phTrackingScripts);
            BindData(SettingName.TrackingEmailTo, chkTrackingEmailTo, phTrackingEmailTo);
            BindData(SettingName.TrackingEmailCc, chkTrackingEmailCc, phTrackingEmailCc);
            BindData(SettingName.TrackingEmailBcc, chkTrackingEmailBcc, phTrackingEmailBcc);
            BindData(SettingName.TrackingEmailReplyTo, chkTrackingEmailReplyTo, phTrackingEmailReplyTo);
            BindData(SettingName.TrackingEmailFrom, chkTrackingEmailFrom, phTrackingEmailFrom);
            BindData(SettingName.TrackingSubject, chkTrackingSubject, phTrackingSubject);
            BindData(SettingName.TrackingMessage, chkTrackingMessage, phTrackingMessage);
            BindData(SettingName.TrackingTriggerOnNew, chkTrackingTriggerOnNew, phTrackingTriggerOnNew);
            BindData(SettingName.TrackingTriggerOnUpdate, chkTrackingTriggerOnUpdate, phTrackingTriggerOnUpdate);
            BindData(SettingName.TrackingTriggerOnDelete, chkTrackingTriggerOnDelete, phTrackingTriggerOnDelete);
            BindData(SettingName.TrackingTextOnNew, chkTrackingTextOnNew, phTrackingTextOnNew);
            BindData(SettingName.TrackingTextOnUpdate, chkTrackingTextOnUpdate, phTrackingTextOnUpdate);
            BindData(SettingName.TrackingTextOnDelete, chkTrackingTextOnDelete, phTrackingTextOnDelete);
        }

        /// <summary>
        /// Bind data for a setting name
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="checkbox"></param>
        /// <param name="placeHolder"></param>
        private void BindData(string settingName, CheckBox checkbox, PlaceHolder placeHolder)
        {
            var settingValue = GetSettingValue(settingName);
            if (!string.IsNullOrEmpty(settingValue))
            {
                placeHolder.Controls.Add(new LiteralControl(settingValue));
                checkbox.Checked = true;
                return;
            }
            checkbox.Checked = false;
        }


        /// <summary>
        /// Read template content from file, then bind to proper placeholder
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="checkBox"></param>
        /// <param name="placeHolder"></param>
        void BindDataForTemplateContent(string settingName, CheckBox checkBox, PlaceHolder placeHolder)
        {
            var relativePath = GetSettingValue(settingName);
            try
            {
                var content = ReadTemplateContentFromFile(relativePath);
                if (!string.IsNullOrEmpty(content))
                {
                    checkBox.Checked = true;
                    placeHolder.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(content)));
                    return;
                }
            }
            catch (Exception)
            {
                checkBox.Checked = false;
            }
            checkBox.Checked = false;
        }

        /// <summary>
        /// Get setting value from ModuleContext.Settings
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        string GetSettingValue(string settingName)
        {
            return ModuleContext.Settings.ContainsKey(settingName)
                ? ModuleContext.Settings[settingName].AsString()
                : null;
        }

        /// <summary>
        /// Read template content from file
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public string ReadTemplateContentFromFile(string relativePath)
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
        /// Build export settings
        /// </summary>
        /// <returns></returns>
        private ExportSettings BuildExportSettings()
        {
            return new ExportSettings
            {
                ModuleSettings = BuildModuleSettings(),
                ModuleConfiguration = BuildModuleConfiguration()
            };
        }

        /// <summary>
        /// Build Module Settings
        /// </summary>
        /// <returns></returns>
        ModuleSettings BuildModuleSettings()
        {
            return new ModuleSettings
            {
                EditOnlyOwnItems = chkEditOwnData.Checked
                    ? GetSettingValue(SettingName.EditOnlyOwnItems)
                    : null,
                EditPrivateColumnsForAdmins = chkPrivateColumns.Checked
                    ? GetSettingValue(SettingName.EditPrivateColumnsForAdmins)
                    : null,
                ForceCaptchaForAnonymous = chkForceCaptcha.Checked
                    ? GetSettingValue(SettingName.ForceCaptchaForAnonymous)
                    : null,
                ShowAllColumnsForAdmins = chkDisplayColumns.Checked
                    ? GetSettingValue(SettingName.ShowAllColumnsForAdmins)
                    : null,
                ShowSystemColumns = chkHideSystemColumns.Checked
                    ? GetSettingValue(SettingName.ShowSystemColumns)
                    : null,
                UserRecordQuota = chkUserRecordQuota.Checked
                    ? GetSettingValue(SettingName.UserRecordQuota)
                    : null,
                ForceInputFiltering = chkInputFiltering.Checked
                    ? GetSettingValue(SettingName.ForceInputFiltering)
                    : null
            };
        }

        /// <summary>
        /// Build Module Configuration
        /// </summary>
        /// <returns></returns>
        ModuleConfiguration BuildModuleConfiguration()
        {
            return new ModuleConfiguration
            {
                CurrentViewOptions = BuildCurrentViewOptions(),
                EmailSettings = BuildEmailSettings(),
                FormSettings = BuildFormSettings(),
                ListSettings = BuildListSettings()
            };
        }

        /// <summary>
        /// Build CurrentViewOptions
        /// </summary>
        /// <returns></returns>
        CurrentViewOptions BuildCurrentViewOptions()
        {
            return new CurrentViewOptions
            {
                Appearance = chkAppearance.Checked
                    ? GetSettingValue(SettingName.ListOrForm)
                    : null,
                UseButtonsInForm = chkUseButtonsInForm.Checked
                    ? GetSettingValue(SettingName.UseButtonsInForm)
                    : null
            };
        }

        /// <summary>
        /// Build Email Settings
        /// </summary>
        /// <returns></returns>
        private EmailSettings BuildEmailSettings()
        {
            return new EmailSettings
            {
                TrackingEmailBcc = chkTrackingEmailBcc.Checked
                    ? GetSettingValue(SettingName.TrackingEmailBcc)
                    : null,
                TrackingEmailCc = chkTrackingEmailCc.Checked
                    ? GetSettingValue(SettingName.TrackingEmailCc)
                    : null,
                TrackingEmailFrom = chkTrackingEmailFrom.Checked
                    ? GetSettingValue(SettingName.TrackingEmailFrom)
                    : null,
                TrackingEmailReplyTo = chkTrackingEmailReplyTo.Checked
                    ? GetSettingValue(SettingName.TrackingEmailReplyTo)
                    : null,
                TrackingEmailTo = chkTrackingEmailTo.Checked
                    ? GetSettingValue(SettingName.TrackingEmailTo)
                    : null,
                TrackingMessage = chkTrackingMessage.Checked
                    ? GetSettingValue(SettingName.TrackingMessage)
                    : null,
                TrackingScripts = chkTrackingScripts.Checked
                    ? GetSettingValue(SettingName.TrackingScript)
                    : null,
                TrackingSubject = chkTrackingSubject.Checked
                    ? GetSettingValue(SettingName.TrackingSubject)
                    : null,
                TrackingTextOnDelete = chkTrackingTextOnDelete.Checked
                    ? GetSettingValue(SettingName.TrackingTextOnDelete)
                    : null,
                TrackingTextOnNew = chkTrackingTextOnNew.Checked
                    ? GetSettingValue(SettingName.TrackingTextOnNew)
                    : null,
                TrackingTextOnUpdate =
                    chkTrackingTextOnUpdate.Checked
                        ? GetSettingValue(SettingName.TrackingTextOnUpdate)
                        : null,
                TrackingTriggerOnDelete = chkTrackingTriggerOnDelete.Checked
                    ? GetSettingValue(SettingName.TrackingTriggerOnDelete)
                    : null,
                TrackingTriggerOnNew = chkTrackingTriggerOnNew.Checked
                    ? GetSettingValue(SettingName.TrackingTriggerOnNew)
                    : null,
                TrackingTriggerOnUpdate = chkTrackingTriggerOnUpdate.Checked
                    ? GetSettingValue(SettingName.TrackingTriggerOnUpdate)
                    : null
            };
        }

        /// <summary>
        /// Build forms settings
        /// </summary>
        /// <returns></returns>
        FormSettings BuildFormSettings()
        {
            return new FormSettings
            {
                SubmissionText = chkSubmissionText.Checked
                    ? GetSettingValue(SettingName.SubmissionText)
                    : null,
                UponSubmitAction = chkUponSubmitAction.Checked
                    ? GetSettingValue(SettingName.UponSubmitAction)
                    : null,
                UponSubmitRedirect = chkUponSubmitRedirect.Checked
                    ? GetSettingValue(SettingName.UponSubmitRedirect)
                    : null
            };
        }

        /// <summary>
        /// Build list settings
        /// </summary>
        /// <returns></returns>
        ListSettings BuildListSettings()
        {
            return new ListSettings
            {
                RenderingMethod = chkRenderingMethod.Checked
                    ? GetSettingValue(SettingName.RenderingMethod)
                    : null,
                SearchSettings = BuildSearchSettings(),
                XslTemplateSettings = BuildXslTemplateSettings(),
                HandlebarsTemplateSettings = BuildHandlebarsTemplateSettings()
            };
        }

        /// <summary>
        /// Build search settings
        /// </summary>
        /// <returns></returns>
        SearchSettings BuildSearchSettings()
        {
            return new SearchSettings
            {
                ExcludeFromSearch = chkExcludeFromSearch.Checked
                    ? GetSettingValue(SettingName.ExcludeFromSearch)
                    : null,
                Filter = chkFilter.Checked
                    ? GetSettingValue(SettingName.Filter)
                    : null,
                Paging = chkPaging.Checked
                    ? GetSettingValue(SettingName.Paging)
                    : null,
                ShowNoRecordsUntilSearch = chkShowNoRecordsUntilSearch.Checked
                    ? GetSettingValue(SettingName.ShowNoRecordsUntilSearch)
                    : null,
                ShowSearchTextBox = chkShowSearchTextBox.Checked
                    ? GetSettingValue(SettingName.ShowSearchTextBox)
                    : null,
                SimpleSearch = chkSimpleSearch.Checked
                    ? GetSettingValue(SettingName.SimpleSearch)
                    : null,
                SortField = chkSortField.Checked
                    ? GetSettingValue(SettingName.SortField)
                    : null,
                SortOrder = chkSortOrder.Checked
                    ? GetSettingValue(SettingName.SortOrder)
                    : null,
                TopCount = chkTopCount.Checked
                    ? GetSettingValue(SettingName.TopCount)
                    : null,
                UrlSearch = chkUrlSearch.Checked
                    ? GetSettingValue(SettingName.URLSearch)
                    : null
            };
        }

        /// <summary>
        /// Build xsl template settings
        /// </summary>
        /// <returns></returns>
        XslTemplateSettings BuildXslTemplateSettings()
        {
            var templateUrl = chkXslTemplateUrl.Checked
                ? GetSettingValue(SettingName.XslUserDefinedStyleSheet)
                : null;
            return new XslTemplateSettings
            {
                TemplateUrl = templateUrl,
                TemplateContent = chkXslTemplateContent.Checked && !string.IsNullOrEmpty(templateUrl)
                    ? ReadTemplateContentFromFile(templateUrl)
                    : null
            };
        }

        /// <summary>
        /// Build handlebars template settings
        /// </summary>
        /// <returns></returns>
        HandlebarsTemplateSettings BuildHandlebarsTemplateSettings()
        {
            var templateUrl = chkHandlebarsTemplateUrl.Checked
                ? GetSettingValue(SettingName.UserDefinedHandlebarsTemplateUrl)
                : null;
            return new HandlebarsTemplateSettings
            {
                RequiredJavaScripts = chkRequiredJavaScripts.Checked
                    ? GetSettingValue(SettingName.UserDefinedHandlebarsTemplateRequiredJavaScripts)
                    : null,
                RequiredStylesheets = chkRequiredStylesheets.Checked
                    ? GetSettingValue(SettingName.UserDefinedHandlebarsTemplateRequiredStylesheets)
                    : null,
                TemplateUrl = templateUrl,
                TemplateContent = chkHandlebarsTemplateContent.Checked && !string.IsNullOrEmpty(templateUrl)
                ? ReadTemplateContentFromFile(templateUrl)
                : null
            };
        }

        #endregion

    }
}