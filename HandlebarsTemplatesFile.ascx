<%@ Control Language="C#" Inherits="DotNetNuke.Modules.UserDefinedTable.HandlebarsTemplatesFile"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" CodeBehind="HandlebarsTemplatesFile.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/Controls/URLControl.ascx" %>

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

        // json format
        $('.json-format').each(function (index, elem) {
            try {
                var jsonObj = JSON.parse(elem.value);
                elem.value = JSON.stringify(jsonObj, '\t', 4);
            } catch (e) {
                console.log('Invalid Json string!');
            }
        });
    });
</script>

<div>
    <div align="right">
        <asp:HyperLink ID="hlpColumns" runat="server" resourcekey="cmdColumnsHelp" NavigateUrl="#"
            CssClass="CommandButton" />
    </div>

    <div class="dnnPanel">
        <h2 class="dnnFormSectionHead">
            <a href="">
                <asp:Label runat="server" ResourceKey="plTemplateEditor" /></a>
        </h2>
        <fieldset>
            <legend></legend>
            <div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lbTemplateContent" ControlName="txtTemplateContent" />
                    <asp:TextBox runat="server" ID="txtTemplateContent" TextMode="MultiLine" Rows="20"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lbRequiredJavaScripts" ControlName="txtRequiredJavaScripts" />
                    <asp:TextBox runat="server" ID="txtRequiredJavaScripts"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lbRequiredStylesheets" ControlName="txtRequiredStylesheets" />
                    <asp:TextBox runat="server" ID="txtRequiredStylesheets"></asp:TextBox>
                </div>
            </div>
        </fieldset>
    </div>

    <div class="dnnPanel">
        <h2 class="dnnFormSectionHead">
            <a href="">
                <asp:Label ID="Label1" runat="server" ResourceKey="plTemplateTest" /></a>
        </h2>
        <fieldset>
            <legend></legend>
            <div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lbSkip" ControlName="txtSkip" />
                    <asp:TextBox runat="server" ID="txtSkip" Text="0"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lbLimit" ControlName="txtLimit" />
                    <asp:TextBox runat="server" ID="txtLimit" Text="10"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" ControlName="btnGetJson" />
                    <asp:Button runat="server" ID="btnGetJson" CssClass="dnnSecondaryAction" Text="Get JSON" />
                    &nbsp;
                    <asp:Button runat="server" ID="btnGetHtml" CssClass="dnnSecondaryAction" Text="Get HTML" />
                </div>

                <div class="dnnFormItem">
                    <div class="dnnForm" id="tabs-demo">
                        <ul class="dnnAdminTabNav">
                            <li>
                                <a href="#jsonContent">
                                    <asp:Label ID="Label3" resourcekey="lbJson" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a href="#htmlContent">
                                    <asp:Label ID="Label4" resourcekey="lbHtml" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a href="#preview">
                                    <asp:Label ID="Label5" resourcekey="lbPreview" runat="server" />
                                </a>
                            </li>
                        </ul>
                        <div id="jsonContent" class="dnnClear tab-content">
                            <asp:TextBox runat="server" ID="txtJson" TextMode="MultiLine" Rows="20" CssClass="tab-content json-format"></asp:TextBox>
                        </div>
                        <div id="htmlContent" class="dnnClear tab-content">
                            <asp:TextBox runat="server" ID="txtHtml" TextMode="MultiLine" Rows="20" CssClass="tab-content"></asp:TextBox>
                        </div>
                        <div id="preview" class="dnnClear tab-content">
                            <asp:Panel runat="server" ID="panelPreview" CssClass="tab-content"></asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </fieldset>
    </div>

    <div class="dnnPanel">
        <h2 class="dnnFormSectionHead">
            <a href="">
                <asp:Label ID="Label6" runat="server" ResourceKey="plTemplateStorage" /></a>
        </h2>
        <fieldset>
            <legend></legend>
            <div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lblFolders" ControlName="ddlFolders" />
                    <asp:DropDownList runat="server" ID="ddlFolders"></asp:DropDownList>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label runat="server" Suffix="" ID="lblFileName" ControlName="txtFileName" />
                    <asp:TextBox runat="server" ID="txtFileName"></asp:TextBox>
                </div>
            </div>
        </fieldset>
    </div>

    <asp:Panel ID="panelError" runat="server" CssClass="dnnFormMessage dnnFormValidationSummary" Visible="False"></asp:Panel>

    <div>
        <asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction"
            Text="Save" BorderStyle="none" CausesValidation="False"></asp:LinkButton>
        <asp:LinkButton ID="cmdBack" resourcekey="cmdBack" runat="server" CssClass="dnnSecondaryAction"
            Text="Back" BorderStyle="none" CausesValidation="False"></asp:LinkButton>
    </div>
</div>


