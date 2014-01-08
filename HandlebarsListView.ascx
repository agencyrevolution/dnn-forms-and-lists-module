<%@ Control Language="C#" Inherits="DotNetNuke.Modules.UserDefinedTable.HandlebarsListView" AutoEventWireup="false" CodeBehind="HandlebarsListView.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnCssInclude runat="server" FilePath="/desktopModules/UserDefinedTable/Styles/pagination.css"></dnn:DnnCssInclude>

<asp:Panel runat="server" ID="panelSearch" CssClass="panel-search">
    <asp:TextBox runat="server" ID="txtSearch" CssClass="search-box"></asp:TextBox>
    <asp:Button runat="server" ID="btnSearch" CssClass="dnnPrimaryAction" Text="Search"/>
</asp:Panel>

<div class="dnnClear">
    <asp:PlaceHolder runat="server" ID="placeHolderMainView"></asp:PlaceHolder>
</div>