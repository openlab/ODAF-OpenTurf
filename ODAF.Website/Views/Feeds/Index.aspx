<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ODDocsMasterPage.Master" Inherits="Views_Feeds_Index" Codebehind="Index.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF REST API for Feeds
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
<link href="<%= ResolveUrl("~/Content/api.css")%>" rel="stylesheet" type="text/css">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div>
   <h1>ODAF REST API for Feeds</h1>

   <div class="api_block">
		<div class="api_url">/Feeds/List</div>
		<span class="api_description">Returns an array of feed sources.</span>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>An array of PointDataSource objects.</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Feeds/{id}/{type}</div>
		<span class="api_description">Returns an atom 2.0 formatted feed of the specified DataPointSource.</span>
        <p>
			<label>Parameters(s):</label>
			<ul>
				<li>{id} (string, required) GUID id of the DataPointSource.</li>
				<li>{type} (string, optional). Can be either "point" or "region". "point" is default.</li>
			</ul>
		</p>
		<p><label>Formats:</label>ATOM 2.0</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>ATOM 2.0 feed</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
     <div class="api_url">PointDataSource object</div>
      <pre>
        {
            "Id":"E2E0A775-D205-4EA1-A444-F10BB0D539E8",
            "Title":"City of Edmonton - Open Data Catalogue"
       }
      </pre>
  </div>
        
</div>
</asp:Content>