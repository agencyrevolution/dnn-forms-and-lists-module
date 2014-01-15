using System;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Modules.UserDefinedTable.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;


namespace DotNetNuke.Modules.UserDefinedTable
{
    public partial class Default : PortalModuleBase, IActionable
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitViews();
        }


        private void InitViews()
        {
            var sec = new ModuleSecurity(ModuleId, TabId, new Components.Settings(Settings));

            switch (ModuleContext.Settings[SettingName.ListOrForm].AsString("Unknown"))
            {
                case "List":
                    var tabSettings = new ModuleController().GetTabModuleSettings(TabModuleId);
                    if (tabSettings.ContainsKey(SettingName.RenderingMethod) &&
                        tabSettings[SettingName.RenderingMethod].ToString().Equals(RenderingMethod.UserDefinedHandlebarsTemplate))
                        LoadControlByKey("HandlebarsListView");
                    else
                        LoadControlByKey("List");
                    break;
                case "Form":
                    if (Request.QueryString["show"].AsString() == "records" && sec.IsAllowedToViewList())
                    {
                        LoadControlByKey("List");
                    }
                    else
                    {
                        LoadControlByKey("Edit");
                    }
                    break;

                case "FormAndList":
                    LoadControlByKey("Edit");
                    if (sec.IsAllowedToViewList())
                    {
                        LoadControlByKey("List");
                    }
                    break;
                case "ListAndForm":
                    if (sec.IsAllowedToViewList())
                    {
                        LoadControlByKey("List");
                    }
                    LoadControlByKey("Edit");
                    break;
                default:
                    LoadControlByKey(IsNewModuleInstance() ? "TemplateList" : "List");
                    break;
            }
        }

        public bool IsNewModuleInstance()
        {
            var settings = ModuleContext.Settings;
            var mc = new ModuleController();
            var minSettings = 0;
            if (settings.ContainsKey(SettingName.ExcludeFromSearch)) minSettings++;
            if (settings.ContainsKey(SettingName.CalculatedColumnsRenderExpressionInForm)) minSettings++;
            if (settings.ContainsKey(SettingName.UseButtonsInForm)) minSettings++;
          
            if (settings.Count > minSettings) return false;
            mc.UpdateModuleSetting( ModuleId,SettingName.UseButtonsInForm, bool.TrueString );
            mc.UpdateModuleSetting(ModuleId, SettingName.ExcludeFromSearch, bool.TrueString);
            mc.UpdateModuleSetting(ModuleId, SettingName.CalculatedColumnsRenderExpressionInForm, bool.TrueString);
            return true;
        }

        void LoadControlByKey(string controlKey)
        {
            var moduleToLoad = ModuleControlController.GetModuleControlByControlKey(controlKey,
                                                                                    ModuleContext.Configuration.
                                                                                        ModuleDefID);
            if (moduleToLoad != null)
            {
                var controlSrc = string.Format("~/{0}", moduleToLoad.ControlSrc);
                var view = (PortalModuleBase) (LoadControl(controlSrc));
                view.ID = Path.GetFileNameWithoutExtension(controlSrc);
                view.ModuleContext.Configuration = ModuleContext.Configuration;
                PlaceHolderControl.Controls.Add(view);
            }
        }

    

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var viewType = ModuleContext.Settings[SettingName.ListOrForm].AsString("Undefined");
                var actions = new ModuleActionCollection();
                foreach (var view in PlaceHolderControl.Controls)
                {
                    if ((view) is IActionable)
                    {
                        var viewactions = ((IActionable) view).ModuleActions;
                        foreach (ModuleAction action in viewactions)
                        {
                            action.ID = GetNextActionID();
                            actions.Add(action);
                        }
                    }
                }
                actions.Add(GetNextActionID(),
                    Localization.GetString(ModuleActionType.ContentOptions, LocalResourceFile),
                    ModuleActionType.ModuleSettings, "", "settings.gif", EditUrl("Manage"), false,
                    SecurityAccessLevel.Edit, true, false);
                if (viewType != "Undefined")
                {
                    actions.Add(GetNextActionID(), Localization.GetString("CreateTemplate.Action", LocalResourceFile),
                        "CreateTemplate", "", Utilities.IconURL("Save"), EditUrl("CreateTemplate"),
                        false, SecurityAccessLevel.Admin, true, false);
                }

                actions.Add(GetNextActionID(), Localization.GetString("ExportCSV.Action", LocalResourceFile),
                    ModuleActionType.ExportModule, "", ResolveUrl("~/images/action_export.gif"),
                    EditUrl("", "", "ExportCSV", "moduleid=" + ModuleId), false,
                    SecurityAccessLevel.Edit, true, false);
                actions.Add(GetNextActionID(), Localization.GetString("ImportCSV.Action", LocalResourceFile),
                    ModuleActionType.ImportModule, "", ResolveUrl("~/images/action_import.gif"),
                    EditUrl("", "", "ImportCSV", "moduleid=" + ModuleId), false,
                    SecurityAccessLevel.Edit, true, false);

                // BEGIN: Handlebars template
                if (IsHandlebarsTemplateSelected())
                {
                    var url = GetEditUrl();

                    actions.Add(GetNextActionID(),
                        Localization.GetString("HandlebarsTemplate.Action", LocalResourceFile),
                        ModuleActionType.EditContent,
                        string.Empty,
                        Utilities.IconURL("Edit"),
                        url,
                        false,
                        SecurityAccessLevel.Edit,
                        true,
                        false);
                }
                // END: Handlebars template

                #region Export/Import Settings

                actions.Add(new ModuleAction(GetNextActionID(),
                    Localization.GetString("ExportModuleSettings.Action", LocalResourceFile),
                    ModuleActionType.ExportModule,
                    string.Empty,
                    Utilities.IconURL("Edit"),
                    EditUrl(ControlKeys.ExportModuleSettings),
                    string.Empty,
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false));

                actions.Add(new ModuleAction(GetNextActionID(),
                    Localization.GetString("ImportModuleSettings.Action", LocalResourceFile),
                    ModuleActionType.ImportModule,
                    string.Empty,
                    Utilities.IconURL("Edit"),
                    EditUrl(ControlKeys.ImportModuleSettings),
                    string.Empty,
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false));
                
                #endregion

                return actions;
            }
        }

        /// <summary>
        /// Get Edit url
        /// </summary>
        /// <returns></returns>
        private string GetEditUrl()
        {
            if (!ModuleContext.Settings.ContainsKey(SettingName.UserDefinedHandlebarsTemplateUrl))
                return EditUrl(ControlKeys.HandlebarsTemplatesFile);
            var relativePath = ModuleContext.Settings[SettingName.UserDefinedHandlebarsTemplateUrl].AsString();
            var file = FileManager.Instance.GetFile(ModuleContext.PortalId, relativePath);
            return file != null
                ? ModuleContext.EditUrl("FileID", file.FileId.ToString(CultureInfo.InvariantCulture),
                    ControlKeys.HandlebarsTemplatesFile)
                : EditUrl(ControlKeys.HandlebarsTemplatesFile);
        }

        /// <summary>
        /// Check if rendering method is Handlebars template or not
        /// </summary>
        /// <returns></returns>
        private bool IsHandlebarsTemplateSelected()
        {
            return ModuleContext.Settings.ContainsKey(SettingName.RenderingMethod) &&
                   ModuleContext.Settings[SettingName.RenderingMethod].ToString().Equals(RenderingMethod.UserDefinedHandlebarsTemplate);
        }
    }
}