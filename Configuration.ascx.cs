using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Modules.UserDefinedTable.Controllers.Caches;
using DotNetNuke.Modules.UserDefinedTable.Interfaces;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.Utilities;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using DataProvider = DotNetNuke.Data.DataProvider;
using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.UserDefinedTable
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The ManageUserDefinedTable Class provides the UI for manageing the UserDefinedTable
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Configuration : ModuleUserControlBase, IActionable, IPostBackEventHandler
    {
// ReSharper disable InconsistentNaming
        protected LabelControl lblNormalizeFlag;
        protected Panel dshRendering;
        protected Panel dshFormsSettings;
        protected Panel dshChangeTracking;
        protected TextEditor teSubmissionSuccess;
        protected TextEditor teTrackingMessage;
        protected LabelControl lblMultipleValues;
        protected LabelControl lblInputSettings;
        protected LabelControl lblDefault;
        protected LabelControl lblOutputSettings;
        protected UrlControl XslUserDefinedUrlControl;
        protected UrlControl XslTracking;
        protected UrlControl urlOnSubmissionRedirect;
        protected UrlControl userDefinedHandlebarsUrlControl;
        // ReSharper restore InconsistentNaming


        ModuleController _objModules;
        const string StrAuto = "[AUTO]";
        protected int RowCount;
        UserDefinedTableController _udtController;

        #region Private Properties

        public ModuleController ModulesController
        {
            get { return _objModules ?? (_objModules = new ModuleController()); }
        }

        UserDefinedTableController UdtController
        {
            get { return _udtController ?? (_udtController = new UserDefinedTableController(ModuleContext)); }
        }
        #endregion

        #region Private Methods

        void BindData()
        {
            BindSettings();
        }
        
        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets the module settings and binds it to the web controls
        /// </summary>
        /// -----------------------------------------------------------------------------
        void BindSettings()
        {
            var oldSelectedSortField = cboSortField.SelectedValue;
            var fields = FieldController.GetFieldsTable(ModuleContext.ModuleId);
            cboSortField.DataSource = new DataView(fields, "UserDefinedFieldId<>\'-1\'", string.Empty,
                                                   DataViewRowState.CurrentRows);
            cboSortField.DataBind();
            cboSortField.Items.Insert(0, new ListItem(Localization.GetString("Not_Specified"), string.Empty));
            if (cboSortField.Items.FindByValue(oldSelectedSortField) != null)
            {
                cboSortField.Items.FindByValue(oldSelectedSortField).Selected = true;
            }
            chkExcludeFromSearch.Checked = ModuleContext.Settings[SettingName.ExcludeFromSearch].AsBoolean();
            cboSortField.ClearSelection();
            var sortFieldSetting = ModuleContext.Settings[SettingName.SortField].AsString();
            if (cboSortField.Items.FindByValue(sortFieldSetting) != null)
            {
                cboSortField.Items.FindByValue(sortFieldSetting).Selected = true;
            }
            cboSortOrder.ClearSelection();
            var sortOrderSetting = ModuleContext.Settings[SettingName.SortOrder].AsString();
            if (cboSortOrder.Items.FindByValue(sortOrderSetting) != null)
            {
                cboSortOrder.Items.FindByValue(sortOrderSetting).Selected = true;
            }

         
            txtFilter.Text = ModuleContext.Settings[SettingName.Filter].AsString();
            txtTopCount.Text = ModuleContext.Settings[SettingName.TopCount].AsString();
            chkShowSearchTextBox.Checked = ModuleContext.Settings[SettingName.ShowSearchTextBox].AsBoolean();
            chkUseButtons.Checked = ModuleContext.Settings[SettingName.UseButtonsInForm].AsBoolean(); 
            chkShowNoRecordsUntilSearch.Checked = ModuleContext.Settings[SettingName.ShowNoRecordsUntilSearch].AsBoolean();
            chkSimpleSearch.Checked = ModuleContext.Settings[SettingName.SimpleSearch].AsBoolean();
            chkURLSearch.Checked = ModuleContext.Settings[SettingName.URLSearch].AsBoolean();

            cboPaging.ClearSelection();
            if (ModuleContext.Settings.ContainsKey(SettingName.Paging))
            {
                if (cboPaging.Items.FindByValue(ModuleContext.Settings[SettingName.Paging].AsString()) != null)
                {
                    cboPaging.SelectedValue = ModuleContext.Settings[SettingName.Paging].AsString();
                }
            }

            var strRenderMethod = ModuleContext.Settings[SettingName.RenderingMethod].AsString();
            if (strRenderMethod == string.Empty)
            {
                strRenderMethod = RenderingMethod.GridRendering;
            }
            renderMethodRadioButtonList.SelectedValue = strRenderMethod;

            try
            {
                if (ModuleContext.Settings.ContainsKey(SettingName.XslUserDefinedStyleSheet))
                {
                    XslUserDefinedUrlControl.Url =
                        ModuleContext.Settings[SettingName.XslUserDefinedStyleSheet].AsString();
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
                //Old StyleSheet removed
            }

            #region Handlebars Template

            if (ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateUrl))
            {
                userDefinedHandlebarsUrlControl.Url = ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateUrl].AsString();
            }

            #endregion

            //Expand sections only, if not all values set to default:
            rblUsageListForm.SelectedValue = ModuleContext.Settings[SettingName.ListOrForm].AsString("List");
            //tracking
            chkTrackingOnDelete.Checked = ModuleContext.Settings[SettingName.TrackingTriggerOnDelete].AsBoolean();
            chkTrackingOnNew.Checked = ModuleContext.Settings[SettingName.TrackingTriggerOnNew].AsBoolean();
            chkTrackingOnUpdate.Checked = ModuleContext.Settings[SettingName.TrackingTriggerOnUpdate].AsBoolean();

            txtOnNew.Text = ModuleContext.Settings[SettingName.TrackingTextOnNew].AsString();
            txtOnUpdate.Text = ModuleContext.Settings[SettingName.TrackingTextOnUpdate].AsString();
            txtOnDelete.Text = ModuleContext.Settings[SettingName.TrackingTextOnDelete].AsString();
            txtTrackingEmailAddresses.Text = ModuleContext.Settings[SettingName.TrackingEmailTo].AsString();
            txtTrackingEmail_CC.Text = ModuleContext.Settings[SettingName.TrackingEmailCc].AsString();
            txtTrackingEmail_BCC.Text = ModuleContext.Settings[SettingName.TrackingEmailBcc].AsString();
            txtTrackingEmail_from.Text = ModuleContext.Settings[SettingName.TrackingEmailFrom].AsString();
            txtTrackingEmail_replyto.Text = ModuleContext.Settings[SettingName.TrackingEmailReplyTo].AsString();
            txtTrackingSubject.Text = ModuleContext.Settings[SettingName.TrackingSubject].AsString();
            var strTrackingScript = ModuleContext.Settings[SettingName.TrackingScript].AsString(StrAuto);
            if (strTrackingScript == StrAuto)
            {
                rblBodyType.SelectedValue = "Auto";
            }
            else
            {
                rblBodyType.SelectedValue = "XslScript";
                try
                {
                    XslTracking.Url = strTrackingScript;
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                    //Old StyleSheet removed
                }
            }

            //form settings
            rblOnSubmission.SelectedValue = ModuleContext.Settings[SettingName.UponSubmitAction].AsString("Form");
            urlOnSubmissionRedirect.Url = ModuleContext.Settings[SettingName.UponSubmitRedirect].AsString();
        }

        bool ValidateMailTo()
        {
            var fields = FieldController.GetFieldsTable(ModuleContext.ModuleId, false);

            return TestForValidEmailSources(fields, txtTrackingEmailAddresses.Text) &&
                   TestForValidEmailSources(fields, txtTrackingEmail_CC.Text) &&
                   TestForValidEmailSources(fields, txtTrackingEmail_BCC.Text) &&
                   TestForValidEmailSources(fields, txtTrackingEmail_replyto.Text) &&
                   TestForValidEmailSources(fields, txtTrackingEmail_from.Text);
        }

        bool TestForValidEmailSources(DataTable fields, string emailSources)
        {
            foreach (var sx in emailSources.Split(';'.ToString(CultureInfo.InvariantCulture).ToCharArray()))
            {
                var source = sx.Trim();
                if (source.Length > 0)
                {
                    if (source.StartsWith("[") && source.EndsWith("]"))
                    {
                        var fieldtitle = source.Substring(1, source.Length - 2);
                        var rows = fields.Select(string.Format("{0}=\'{1}\'", FieldsTableColumn.Title, fieldtitle));
                        if (rows.Length == 1)
                        {
                            var type = DataType.ByName((string) (rows[0][FieldsTableColumn.Type]));
                            if (!(type is IEmailAdressSource))
                            {
                                UI.Skins.Skin.AddModuleMessage(this,
                                                               string.Format(
                                                                   Localization.GetString("NotEmailSource",
                                                                                          LocalResourceFile), fieldtitle),
                                                               ModuleMessage.ModuleMessageType.YellowWarning);
                                return false;
                            }
                        }
                        else if (rows.Length == 0)
                        {
                            UI.Skins.Skin.AddModuleMessage(this,
                                                           string.Format(
                                                               Localization.GetString("NotEmailSource",
                                                                                      LocalResourceFile), fieldtitle),
                                                           ModuleMessage.ModuleMessageType.YellowWarning);
                            return false;
                        }
                    }
                    else
                    {
                        if (! Mail.IsValidEmailAddress(source, ModuleContext.PortalId))
                        {
                            UI.Skins.Skin.AddModuleMessage(this,
                                                           string.Format(
                                                               Localization.GetString("NotEmailAddress",
                                                                                      LocalResourceFile), source),
                                                           ModuleMessage.ModuleMessageType.YellowWarning);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Set the visibility of controls according to
        /// </summary>
        /// -----------------------------------------------------------------------------
        void ApplySettingsVisibility()
        {
            var isFormMode = rblUsageListForm.SelectedValue == "Form";
            rememberSettings.Visible = rblUsageListForm.SelectedValue.Contains("Form");
            plainFormSettingSet.Visible = isFormMode;
            noFormSettings.Visible = !isFormMode;
            rowOnSubmissionRedirect.Visible = rblOnSubmission.SelectedValue == "Redirect";
            rowSubmissionText.Visible = Convert.ToBoolean(! rowOnSubmissionRedirect.Visible);
        
            rowTrackingScript.Visible = Convert.ToBoolean(rblBodyType.SelectedValue == "XslScript");

            switch (renderMethodRadioButtonList.SelectedValue)
            {
                case RenderingMethod.GridRendering:
                    rowUserDefined.Visible = false;
                    handlebarsTemplatePanel.Visible = false;
                    break;
             
                case RenderingMethod.UserdefinedXSL:
                    rowUserDefined.Visible = true;
                    handlebarsTemplatePanel.Visible = false;
                    break;
                case RenderingMethod.UserDefinedHandlebarsTemplate:
                    rowUserDefined.Visible = false;
                    handlebarsTemplatePanel.Visible = true;
                    break;
            }
        }

        int GetEditableFileId(string url)
        {
            if (LikeOperator.LikeString(url, "fileid=*", CompareMethod.Binary))
            {
                var fileId = int.Parse(UrlUtils.GetParameterValue(url));
                var f = FileManager.Instance.GetFile(fileId);
                if (Utilities.HasWritePermission(f.Folder, ModuleContext.PortalId))
                {
                    return fileId;
                }
            }
            return - 1;
        }

        void UpdateModuleSetting(string name, string value)
        {
            ModulesController.UpdateModuleSetting(ModuleContext.ModuleId, name, value);
            ModuleContext.Settings[name] = value;
        }

        void UpdateTabModuleSetting(string name, string value)
        {
            ModulesController.UpdateTabModuleSetting(ModuleContext.TabModuleId, name, value);
            ModuleContext.Settings[name] = value;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        void SaveSettings()
        {
            try
            {
                // clear html cache
                new CachedHtmlContentController().DeleteCachedHtmlContentsByModuleId(ModuleContext.ModuleId);

                UpdateModuleSetting(SettingName.ExcludeFromSearch, chkExcludeFromSearch.Checked.ToString(CultureInfo.InvariantCulture));

                if (cboSortField.SelectedItem != null)
                {
                    var sortOrder = cboSortField.SelectedIndex == 0
                                        ? string.Empty
                                        : cboSortOrder.SelectedItem.Value.AsString("ASC");
                    UpdateTabModuleSetting(SettingName.SortOrder, sortOrder);
                    UpdateTabModuleSetting(SettingName.SortField, cboSortField.SelectedItem.Value);
                }
                else
                {
                    UpdateTabModuleSetting(SettingName.SortOrder, string.Empty);
                    UpdateTabModuleSetting(SettingName.SortField, string.Empty);
                }
                
                //save rendering and rendering specific values
                var strRenderingMethod = renderMethodRadioButtonList.SelectedValue;
                var strUserDefinedXsl = "";
                if (strRenderingMethod == RenderingMethod.UserdefinedXSL && XslUserDefinedUrlControl.Url != string.Empty)
                {
                    var file = FileManager.Instance.GetFile(int.Parse(XslUserDefinedUrlControl.Url.Substring(7)));
                    if (file == null)
                    {
                        strRenderingMethod = RenderingMethod.GridRendering;
                    }
                    else
                    {
                        strUserDefinedXsl = file.Folder + file.FileName;
                    }
                } 
                
                // BEGIN: Handlebars template
                if (strRenderingMethod == RenderingMethod.UserDefinedHandlebarsTemplate)
                {
                    var file = FileManager.Instance.GetFile(int.Parse(userDefinedHandlebarsUrlControl.Url.Substring(7)));
                    if (file == null)
                    {
                        strRenderingMethod = RenderingMethod.GridRendering;
                    }
                    else
                    {
                        UpdateTabModuleSetting(SettingName.UserDefinedHandlebarsTemplateUrl, file.Folder + file.FileName);
                    }
                }
                // END: Handlebars template

                UpdateTabModuleSetting(SettingName.XslUserDefinedStyleSheet, strUserDefinedXsl);
                UpdateTabModuleSetting(SettingName.RenderingMethod, strRenderingMethod);
                //Paging
                UpdateTabModuleSetting(SettingName.Paging, cboPaging.SelectedValue);
                //Filter
                UpdateTabModuleSetting(SettingName.Filter, txtFilter.Text);
                if (txtTopCount.Text == string.Empty)
                {
                    UpdateModuleSetting(SettingName.TopCount, string.Empty);
                }
                else if (Information.IsNumeric(txtTopCount.Text))
                {
                    var count = int.Parse(txtTopCount.Text);
                    if (count < 1)
                    {
                        count = 1;
                    }
                    if (count > 1000)
                    {
                        count = 1000;
                    }
                    UpdateModuleSetting(SettingName.TopCount, count.ToString(CultureInfo.InvariantCulture));
                }
                UpdateTabModuleSetting(SettingName.TopCount, txtTopCount.Text);
                //Search
                UpdateTabModuleSetting(SettingName.ShowSearchTextBox, chkShowSearchTextBox.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.ShowNoRecordsUntilSearch, chkShowNoRecordsUntilSearch.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.SimpleSearch, chkSimpleSearch.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.UseButtonsInForm, chkUseButtons.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.URLSearch, chkURLSearch.Checked.ToString(CultureInfo.InvariantCulture));
               //forms or list
                UpdateTabModuleSetting(SettingName.ListOrForm, rblUsageListForm.SelectedValue);
                //form
                UpdateTabModuleSetting(SettingName.SubmissionText, HttpUtility.HtmlDecode(teSubmissionSuccess.Text));
                UpdateTabModuleSetting(SettingName.UponSubmitRedirect, urlOnSubmissionRedirect.Url);
                UpdateTabModuleSetting(SettingName.UponSubmitAction, rblOnSubmission.SelectedValue);
                //tracking
                var strTrackingEmailSkript = "";

                if (rblBodyType.SelectedValue == "XslScript" && XslTracking.Url != string.Empty)
                {
                    var file = FileManager.Instance.GetFile(int.Parse(XslTracking.Url.Substring(7)));
                    if (file != null)
                    {
                        strTrackingEmailSkript = file.Folder + file.FileName;
                    }
                }
                else
                {
                    strTrackingEmailSkript = StrAuto;
                }

                UpdateTabModuleSetting(SettingName.TrackingScript, strTrackingEmailSkript);
                UpdateTabModuleSetting(SettingName.TrackingEmailTo, txtTrackingEmailAddresses.Text);
                UpdateTabModuleSetting(SettingName.TrackingEmailCc, txtTrackingEmail_CC.Text);
                UpdateTabModuleSetting(SettingName.TrackingEmailBcc, txtTrackingEmail_BCC.Text);
                UpdateTabModuleSetting(SettingName.TrackingEmailReplyTo, txtTrackingEmail_replyto.Text);
                UpdateTabModuleSetting(SettingName.TrackingEmailFrom, txtTrackingEmail_from.Text);
                UpdateTabModuleSetting(SettingName.TrackingSubject, txtTrackingSubject.Text);
                UpdateTabModuleSetting(SettingName.TrackingMessage, HttpUtility.HtmlDecode(teTrackingMessage.Text));
                UpdateTabModuleSetting(SettingName.TrackingTriggerOnDelete, chkTrackingOnDelete.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.TrackingTriggerOnNew, chkTrackingOnNew.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.TrackingTriggerOnUpdate, chkTrackingOnUpdate.Checked.ToString(CultureInfo.InvariantCulture));
                UpdateTabModuleSetting(SettingName.TrackingTextOnNew, txtOnNew.Text);
                UpdateTabModuleSetting(SettingName.TrackingTextOnUpdate, txtOnUpdate.Text);
                UpdateTabModuleSetting(SettingName.TrackingTextOnDelete, txtOnDelete.Text);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        void SetHelpLinks()
        {
            var helpUrL = string.Format("javascript:OpenHelpWindow(\'{0}\')",
                                        ResolveUrl(
                                            "~/desktopmodules/userdefinedtable/HelpPopup.aspx?resourcekey=Help_Tokens_6"));
            hlpToken2.NavigateUrl = helpUrL;
            helpUrL = string.Format("javascript:OpenHelpWindow(\'{0}\')",
                                    ResolveUrl(
                                        "~/desktopmodules/userdefinedtable/HelpPopup.aspx?resourcekey=Help_HiddenColumns"));
            hlpColumns2.NavigateUrl = helpUrL;
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            // Moved from  BindSettings to handle issues with the RTE
            teSubmissionSuccess.Text =
                ModuleContext.Settings[SettingName.SubmissionText].AsString(Localization.GetString("SubmissionSuccess",
                                                                                                   LocalResourceFile));
            teTrackingMessage.Text = ModuleContext.Settings[SettingName.TrackingMessage].AsString();
            cmdCancel.Click += cmdCancel_Click;
           
            cmdEditEmail.Click += cmdEditEmail_Click;
            cmdEditXSL.Click += cmdEditXSL_Click;
            cmdGenerateEmail.Click += cmdGenerateEmail_Click;
            cmdGenerateXSL.Click += cmdGenerateXSL_Click;
            cmdUpdate.Click += cmdUpdate_Click;
            chkShowSearchTextBox.CheckedChanged += chkShowSearchTextBox_CheckedChanged;
          
            // BEGIN: Handlebars template
            cmdEditHandlebars.Click += cmdEditHandlebars_Click;
            cmdGenerateHandlebars.Click += cmdGenerateHandlebars_Click;
            // END: Handlebars template

            Load += Page_Load;
            Fields.LocalizeString = LocalizeString;
            Fields.LocalResourceFile = LocalResourceFile;
            Fields.ModuleContext = ModuleContext;
            jQuery.RequestDnnPluginsRegistration();
            ClientAPI.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);
        }

        void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (! Page.IsPostBack)
                {
                    //Localize Grid
                    BindData();
                }
                SetHelpLinks();
                ApplySettingsVisibility();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #region Handlebars template

        /// <summary>
        /// Triggered when user clicks on Generate New button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdGenerateHandlebars_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Response.Redirect(ModuleContext.EditUrl(ControlKeys.HandlebarsTemplatesFile), true);
        }

        /// <summary>
        /// Triggered when user clicks on Edit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdEditHandlebars_Click(object sender, EventArgs e)
        {
            var fileId = GetEditableFileId(userDefinedHandlebarsUrlControl.Url.ToLowerInvariant());
            if (fileId > -1)
            {
                SaveSettings();
                Response.Redirect(ModuleContext.EditUrl("FileID", fileId.ToString(CultureInfo.InvariantCulture), ControlKeys.HandlebarsTemplatesFile), true);
            }
        }

        #endregion

        void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(ModuleContext.TabId), true);
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (ValidateMailTo())
            {
                if (chkExcludeFromSearch.Checked)
                {
                    DataProvider.Instance().DeleteSearchItems(ModuleContext.ModuleId);
                }
                SaveSettings();
                Response.Redirect(Globals.NavigateURL(ModuleContext.TabId), true);
            }
        }
        
        void cmdGenerateXSL_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Response.Redirect(ModuleContext.EditUrl("GenerateXsl"), true);
        }

        void cmdEditXSL_Click(object sender, EventArgs e)
        {
            var fileId = GetEditableFileId(XslUserDefinedUrlControl.Url.ToLowerInvariant());
            if (fileId > - 1)
            {
                SaveSettings();
                Response.Redirect(ModuleContext.EditUrl("FileID", fileId.ToString(CultureInfo.InvariantCulture), "GenerateXsl"), true);
            }
        }

        void chkShowSearchTextBox_CheckedChanged(object sender, EventArgs e)
        {
            var searchEnabled = chkShowSearchTextBox.Checked;
            chkShowNoRecordsUntilSearch.Enabled = searchEnabled;
            chkSimpleSearch.Enabled = searchEnabled;
            chkURLSearch.Enabled = searchEnabled;
            if (! searchEnabled)
            {
                chkShowNoRecordsUntilSearch.Checked = false;
                chkSimpleSearch.Checked = false;
                chkURLSearch.Checked = false;
            }
        }

        protected void cmdEditEmail_Click(object sender, EventArgs e)
        {
            var fileId = GetEditableFileId(XslTracking.Url.ToLowerInvariant());
            if (fileId > - 1)
            {
                SaveSettings();
                Response.Redirect(ModuleContext.EditUrl("FileID", fileId.ToString(CultureInfo.InvariantCulture), "GenerateXsl"), true);
            }
        }

        protected void cmdGenerateEmail_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Response.Redirect(ModuleContext.EditUrl("tracking", "true", "GenerateXsl"), true);
        }

        #endregion

        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();
                var url = string.Format("javascript:{0}",
                                        Page.ClientScript.GetPostBackEventReference(this, "SaveSettingsAsDefault"));
                actions.Add(ModuleContext.GetNextActionID(),
                            Localization.GetString("SaveSettingsAsDefault.Action", LocalResourceFile), "", "",
                            Utilities.IconURL("Save"), url, false, SecurityAccessLevel.Edit, true, false);

                url = string.Format("javascript:if (confirm(\'{0}\')) {1}",
                                    Localization.GetString("ResetModuleTabSettings.Confirm",
                                                           LocalResourceFile).AsString().Replace("\'", "\\\'"),
                                    Page.ClientScript.GetPostBackEventReference(this, "ResetModuleTabSettings"));
                actions.Add(ModuleContext.GetNextActionID(),
                            Localization.GetString("ResetModuleTabSettings.Action", LocalResourceFile), "", "",
                            Utilities.IconURL("Restore"), url, false, SecurityAccessLevel.Edit, true, false);

                url = string.Format("javascript:if (confirm(\'{0}\')) {1}",
                                    Localization.GetString("DeleteAll.Confirm", LocalResourceFile).AsString().
                                        Replace("\'", "\\\'"),
                                    Page.ClientScript.GetPostBackEventReference(this, "DeleteAll"));
                actions.Add(ModuleContext.GetNextActionID(),
                            Localization.GetString("DeleteAll.Action", LocalResourceFile), "", "",
                            Utilities.IconURL("Save"), url, false, SecurityAccessLevel.Edit, true, false);

                #region Export/Import Settings

                actions.Add(new ModuleAction(ModuleContext.GetNextActionID(),
                    Localization.GetString("ExportModuleSettings.Action", LocalResourceFile),
                    ModuleActionType.ExportModule,
                    string.Empty,
                    Utilities.IconURL("Edit"),
                    ModuleContext.EditUrl(ControlKeys.ExportModuleSettings),
                    string.Empty,
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false));

                actions.Add(new ModuleAction(ModuleContext.GetNextActionID(),
                    Localization.GetString("ImportModuleSettings.Action", LocalResourceFile),
                    ModuleActionType.ImportModule,
                    string.Empty,
                    Utilities.IconURL("Edit"),
                    ModuleContext.EditUrl(ControlKeys.ImportModuleSettings),
                    string.Empty,
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false));

                #endregion

                return actions;
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            switch (eventArgument)
            {
                case "ResetModuleTabSettings":
                    ModulesController.DeleteTabModuleSettings(ModuleContext.TabModuleId);
                    break;
                case "SaveSettingsAsDefault":
                    SaveSettings();
                    var tabModuleSettings = ModulesController.GetTabModuleSettings(ModuleContext.TabModuleId);

                    foreach (string key in tabModuleSettings.Keys)
                    {
                        ModulesController.UpdateModuleSetting(ModuleContext.ModuleId, key,
                                                              tabModuleSettings[key].ToString());
                    }
                    break;
                case "DeleteAll":
                    UdtController.DeleteRows();
                    Response.Redirect(Globals.NavigateURL(ModuleContext.TabId), true);
                    break;
            }
        }

        #endregion
    }
}