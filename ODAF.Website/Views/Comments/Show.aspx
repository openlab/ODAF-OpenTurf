<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ODMasterPage.Master" CodeBehind="Show.aspx.cs" Inherits="Views_Comments_Show" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="vancouveropendata.Controllers" %>
<%@ Import Namespace="System.Configuration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
<%=website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string%> comment by <%=ViewData.Model.CommentAuthor %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
    <meta name="viewport" content="width = device-width, initial-scale = 1.0, maximum-scale = 1.0, user-scalable = 0" />
    <style type="text/css">
        
        body
        {
            background:#40225F url(<%= ResolveUrl("~/images/background-tile-vertical.png")%>) repeat-y;
            font-family: Helvetica;
            font-size:10pt;
            color:#FFF;
            padding:0;
            margin:0;
        }
      
        #avatar
        {
            height: 73px;
            width: 73px;
            float: left;
            padding-right: 10px;
        }

        #logo
        {
        }

        #main-box
        {
        	margin-top: 20px;
            margin-right: auto;
            margin-left: auto;
            <% if (Request.Browser.IsMobileDevice) { %>
                width: <%=Request.Browser.ScreenPixelsWidth%>px;
            <% } else { %>
                width: 500px;
            <% } %>
            height: 100%;
        }

        #comment-box
        {
        	margin-top: 20px;
            background-color: White;
            color: Black;
            padding: 20px;
        }

        #comment
        {
            font-size: 1.5em;
        }

        #author
        {
            margin-top: 10px;
            height: 70px;
        }

        #name
        {
            vertical-align: middle;
            font-size: 1.2em;
            padding-top: 10px;
        }
        
        #date
        {
        	font-size: 0.9em;
        	color:#c0c0c0;
        }
        
        #app-link
        {
        	text-decoration: none;
        	font-size: 1.2em;
        	color:#c0c0c0;
        	float:right;
        }
        
        #location a
        {
        	text-decoration: none;
        	font-size: 0.8em;
        	color:#c0c0c0;
        }
        
      </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<div id="main-box">
<img alt="<%=website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string%> logo" id="logo" src="<%=ResolveUrl("~/images/logo.png") %>"/>
<% if (Request.Browser.IsMobileDevice && Request.Browser.Platform.Equals("iPhone OS"))
   { %>
    <a id="app-link" href="<%=(website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string).ToLower()%>://ViewComment/?<%=String.Format("SummaryId={0}&CommentId={1}", ViewData.Model.Comment.SummaryId, ViewData.Model.Comment.Id) %>">go to app</a>
<% }
   else
   { %>
    <a id="app-link" href="<%=ResolveUrl("~/") %>">go to app</a>
<% } %>
<div style="clear:both"></div>

<div id="comment-box">
    <div id="comment">
        <%=Html.Encode(ViewData.Model.Comment) %>
    </div>
    <div id="author">
        <img alt="User Image" id="avatar" src="<%=this.GetUserImageUrl(ViewData.Model.Comment.CreatedById) %>" />
        <div id="name"><%=ViewData.Model.CommentAuthor %></div>
        <div id="date"><%=ViewData.Model.Comment.CreatedOn.ToString() %></div>
        <div id="location">
            <%=Html.ActionLink<SummariesController>(c => c.ShowById(ViewData.Model.Comment.SummaryId, "html"), 
                ViewData.Model.Comment.PointDataSummaries.Single().Name)%>
         </div>
    </div>
</div>
</div>

</asp:Content>