<%@ Control Language="C#" Inherits="DotNetNuke.Modules.UserDefinedTable.ImportModuleSettings"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="ImportModuleSettings.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:DnnCssInclude runat="server" FilePath="/desktopModules/UserDefinedTable/Styles/importModuleSettings.css"></dnn:DnnCssInclude>

<script type="text/javascript">
    jQuery(function ($) {

        $('#tabs-demo').dnnTabs();
        $('.tab-content').css('max-width', '98%')
            .css('width', '98%');

        var setupModule = function () {
            $('.dnnPanel').dnnPanels();
            $('.dnnFormExpandContent a').dnnExpandAll({
                targetArea: '#dnnPanel'
            });
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });
</script>

<div>
    <div align="right">
        <asp:HyperLink ID="hlpColumns" runat="server" resourcekey="cmdColumnsHelp" NavigateUrl="#"
            CssClass="CommandButton" />
    </div>

    <p>By default, all settings, which are not conflicted with the current settings, will be imported. You can also choose which settings you want to force override.</p>

    <div class="dnnFormItem">
        <dnn:Label runat="server" Suffix="" ID="lblFileImport" ControlName="fileImport" />
        <asp:FileUpload runat="server" ID="fileImport"></asp:FileUpload>
        <asp:Button runat="server" ID="btnUpload" Text="Upload" CssClass="dnnPrimaryAction" />
    </div>

    <asp:Panel runat="server" ID="panelPreview" Visible="False">

        <!-- Module Settings -->
        <div class="dnnPanel">
            <h2 class="dnnFormSectionHead">
                <a href="">
                    <asp:Label ID="Label1" runat="server" ResourceKey="plModuleSettings" /></a>
            </h2>
            <fieldset>
                <legend></legend>
                <table class="dnnGrid">
                    <tbody>
                        <tr class="dnnGridHeader" valign="top">
                            <th></th>
                            <th>Setting Name</th>
                            <th>Current Setting Value</th>
                            <th>New Setting Value</th>
                        </tr>

                        <!-- User is only allowed to manipulate his own items. -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkEditOwnData"
                                    runat="server"
                                    CssClass="Normal"></asp:CheckBox>
                            </td>
                            <td class="second-column">User is only allowed to manipulate his own items.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phEditOwnData" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phEditOwnDataNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Force Captcha control during edit for Anonymous to fight spam. -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkForceCaptcha"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Force Captcha control during edit for Anonymous to fight spam.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phForceCaptcha" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phForceCaptchaNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Filter input for markup code or script input. Note: Filtering is always enabled for Anonymous. -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkInputFiltering"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Filter input for markup code or script input. Note: Filtering is always enabled for Anonymous.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phInputFiltering" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phInputFilteringNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Negate permission/feature 'DisplayColumns' for Administators. -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkDisplayColumns"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Negate permission/feature 'DisplayColumns' for Administators.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phDisplayColumns" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phDisplayColumnsNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Hide System Fields even if 'Display All Column' permission is set. -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkHideSystemColumns"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Hide System Fields even if 'Display All Column' permission is set.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phHideSystemColumns" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phHideSystemColumnsNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Negate permission/feature 'PrivateColumns' for Administators. -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkPrivateColumns"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Negate permission/feature 'PrivateColumns' for Administators.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phPrivateColumns" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phPrivateColumnsNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Maximum Records per User. -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkUserRecordQuota"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Maximum Records per User.</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phUserRecordQuota" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phUserRecordQuotaNew" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </fieldset>
        </div>

        <!-- Current View Options -->
        <div class="dnnPanel">
            <h2 class="dnnFormSectionHead">
                <a href="">
                    <asp:Label ID="Label2" runat="server" ResourceKey="plCurrentViewOptions" /></a>
            </h2>
            <fieldset>
                <legend></legend>
                <table class="dnnGrid">
                    <tbody>
                        <tr class="dnnGridHeader" valign="top">
                            <th></th>
                            <th>Setting Name</th>
                            <th>Current Setting Value</th>
                            <th>New Setting Value</th>
                        </tr>
                        <!-- Appearance -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkAppearance"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Appearance</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phAppearance" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phAppearanceNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Use Buttons In Form -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkUseButtonsInForm"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Use Buttons In Form</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phUseButtonsInForm" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phUseButtonsInFormNew" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </fieldset>
        </div>

        <!-- Form Settings -->
        <div class="dnnPanel">
            <h2 class="dnnFormSectionHead">
                <a href="">
                    <asp:Label ID="Label3" runat="server" ResourceKey="plFormSettings" /></a>
            </h2>
            <fieldset>
                <legend></legend>
                <table class="dnnGrid">
                    <tbody>
                        <tr class="dnnGridHeader" valign="top">
                            <th></th>
                            <th>Setting Name</th>
                            <th>Current Setting Value</th>
                            <th>New Setting Value</th>
                        </tr>
                        <!-- Action upon submit -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkUponSubmitAction"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Action upon submit</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phUponSubmitAction" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phUponSubmitActionNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Upon Submit Redirect -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkUponSubmitRedirect"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Upon Submit Redirect</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phUponSubmitRedirect" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phUponSubmitRedirectNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Displayed text after form post -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkSubmissionText"
                                    ResourceKey="SubmissionText"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Displayed text after form post</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phSubmissionText" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phSubmissionTextNew" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </fieldset>
        </div>

        <!-- List Settings -->
        <div class="dnnPanel">
            <h2 class="dnnFormSectionHead">
                <a href="">
                    <asp:Label ID="Label4" runat="server" ResourceKey="plListSettings" /></a>
            </h2>
            <fieldset>
                <legend></legend>
                <table class="dnnGrid">
                    <tbody>
                        <tr class="dnnGridHeader" valign="top">
                            <th></th>
                            <th>Setting Name</th>
                            <th>Current Setting Value</th>
                            <th>New Setting Value</th>
                        </tr>
                        <!-- Rendering Method -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkRenderingMethod"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Rendering Method</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phRenderingMethod" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phRenderingMethodNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Search Options -->
                        <tr class="dnnGridAltItem">
                            <td colspan="4"><strong>Search Options</strong></td>
                        </tr>
                        <!-- Exclude From Search -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkExcludeFromSearch"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Exclude From Search</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phExcludeFromSearch" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phExcludeFromSearchNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Sort Order -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkSortOrder"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Sort Order</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phSortOrder" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phSortOrderNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Sort Field -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkSortField"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Sort Field</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phSortField" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phSortFieldNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Paging -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkPaging"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Paging</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phPaging" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phPagingNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Filter -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkFilter"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Filter</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phFilter" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phFilterNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Top Count -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTopCount"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Top Count</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTopCount" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTopCountNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Show Search TextBox -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkShowSearchTextBox"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Show Search TextBox</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phShowSearchTextBox" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phShowSearchTextBoxNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Show No Records Until Search -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkShowNoRecordsUntilSearch"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Show No Records Until Search</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phShowNoRecordsUntilSearch" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phShowNoRecordsUntilSearchNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Simple Search -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkSimpleSearch"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Simple Search</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phSimpleSearch" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phSimpleSearchNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Url Search -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkUrlSearch"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Url Search</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phUrlSearch" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phUrlSearchNew" runat="server" />
                            </td>
                        </tr>

                        <!-- XSL Template Settings -->
                        <tr class="dnnGridItem">
                            <td colspan="4"><strong>XSL Template Settings</strong></td>
                        </tr>
                        <!-- XSL Template Url -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkXslTemplateUrl"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">XSL Template Url</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phXslTemplateUrl" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phXslTemplateUrlNew" runat="server" />
                            </td>
                        </tr>
                        <!-- XSL Template Content -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkXslTemplateContent"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Xsl Template Content</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phXslTemplateContent" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phXslTemplateContentNew" runat="server" />
                            </td>
                        </tr>

                        <!-- Handlebars Template Settings -->
                        <tr class="dnnGridAltItem">
                            <td colspan="4"><strong>Handlebars Template Options</strong></td>
                        </tr>
                        <!-- Handlebars Template Url -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkHandlebarsTemplateUrl"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Handlebars Template Url</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phHandlebarsTemplateUrl" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phHandlebarsTemplateUrlNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Handlebars Template Content -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkHandlebarsTemplateContent"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Handlebars Template Content</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phHandlebarsTemplateContent" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phHandlebarsTemplateContentNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Required JavaScripts -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkRequiredJavaScripts"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Required JavaScripts</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phRequiredJavaScripts" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phRequiredJavaScriptsNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Required Stylesheets -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkRequiredStylesheets"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Required Stylesheets</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phRequiredStylesheets" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phRequiredStylesheetsNew" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </fieldset>
        </div>

        <!-- Email Settings -->
        <div class="dnnPanel">
            <h2 class="dnnFormSectionHead">
                <a href="">
                    <asp:Label ID="Label5" runat="server" ResourceKey="plEmailSettings" /></a>
            </h2>
            <fieldset>
                <legend></legend>
                <table class="dnnGrid">
                    <tbody>
                        <tr class="dnnGridHeader" valign="top">
                            <th></th>
                            <th>Setting Name</th>
                            <th>Current Setting Value</th>
                            <th>New Setting Value</th>
                        </tr>
                        <!-- Tracking Scripts -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingScripts"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Scripts</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingScripts" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingScriptsNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Email To -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingEmailTo"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Email To</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingEmailTo" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingEmailToNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Email CC -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingEmailCc"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Email CC</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingEmailCc" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingEmailCcNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Email BCC -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingEmailBcc"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Email BCC</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingEmailBcc" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingEmailBccNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Email Reply To -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingEmailReplyTo"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Email Reply To</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingEmailReplyTo" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingEmailReplyToNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Email From -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingEmailFrom"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Email From</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingEmailFrom" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingEmailFromNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Subject -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingSubject"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Subject</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingSubject" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingSubjectNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Message -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingMessage"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Message</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingMessage" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingMessageNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Trigger On New -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingTriggerOnNew"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Trigger On New</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingTriggerOnNew" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingTriggerOnNewNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Trigger On Update -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingTriggerOnUpdate"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Trigger On Update</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingTriggerOnUpdate" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingTriggerOnUpdateNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Trigger On Delete -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingTriggerOnDelete"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Trigger On Delete</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingTriggerOnDelete" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingTriggerOnDeleteNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Text On New -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingTextOnNew"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Text On New</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingTextOnNew" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingTextOnNewNew" runat="server" />
                            </td>
                        </tr>
                        <!-- Tracking Text On Update -->
                        <tr class="dnnGridItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingTextOnUpdate"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Text On Update</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingTextOnUpdate" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingTextOnUpdateNew" runat="server" />
                            </td>
                        </tr>
                        <!--Tracking Text On Delete -->
                        <tr class="dnnGridAltItem">
                            <td class="first-column">
                                <asp:CheckBox ID="chkTrackingTextOnDelete"
                                    CssClass="Normal"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="second-column">Tracking Text On Delete</td>
                            <td class="third-column">
                                <asp:PlaceHolder ID="phTrackingTextOnDelete" runat="server" />
                            </td>
                            <td class="forth-column">
                                <asp:PlaceHolder ID="phTrackingTextOnDeleteNew" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </fieldset>
        </div>

    </asp:Panel>

    <asp:Panel ID="panelError" runat="server" CssClass="dnnFormMessage dnnFormValidationSummary" Visible="False"></asp:Panel>
    
    <hr/>

    <p>
        <asp:LinkButton ID="cmdImport" runat="server" CssClass="dnnPrimaryAction"
            Text="Import Module Settings" BorderStyle="none" CausesValidation="False" Visible="False"></asp:LinkButton>
        <asp:LinkButton ID="cmdBack" resourcekey="cmdBack" runat="server" CssClass="dnnSecondaryAction"
            Text="Back" BorderStyle="none" CausesValidation="False"></asp:LinkButton>
    </p>
</div>


