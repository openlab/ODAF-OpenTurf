<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ODDocsMasterPage.Master" Inherits="Views_Comments_Index" Codebehind="Index.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF REST API for Comments
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
<link href="<%= ResolveUrl("~/Content/api.css")%>" rel="stylesheet" type="text/css">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div>
   <h1>ODAF REST API for Comments</h1>
   
   <div class="api_block">
		<div class="api_url">/Comments/Show.{format}/{commentId}</div>
		<span class="api_description">Gets a comment (aggregated with CommentAuthor and ServiceName) by comment id. </span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{commentId}. (unsigned integer)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json, html</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>json. an AggregatedComment object.</li>
				<li>html. shows the AggregatedComment in a html page.</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Comments/List.{format}/{summaryId}</div>
		<span class="api_description">Returns the list of comments (aggregated with CommentAuthor and ServiceName) for a PointDataSummary</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{summaryId}. (unsigned integer).</li>
			</ul>
		</p>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>page. (integer, optional) First page is 0, and default value is 0.</li>
				<li>page_size. (integer, optional)</li>
			</ul>
		</p>
	<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>list of AggregatedComment objects.</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Comments/Add.{format}/{summaryId}</div>
		<span class="api_description">Add the comment to the PointDataSummary</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{summaryId}. (unsigned integer)</li>
			</ul>
		</p>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>Text. (string)</li>
			</ul>
		</p>
	<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>HTTP Method(s):</label>POST, PUT</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>201: Success. Returns the newly created PointDataComment object. Also adds a Location: header with the URL of the newly created resource.</li>
				<li>400: Error.</li>
			</ul>
		</p>     </div>

   <div class="api_block">
		<div class="api_url">/Comments/Remove.{format}/{commentId}</div>
		<span class="api_description"></span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{commentId}. (unsigned integer)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>Minimum role required:</label>Administrator</p>
		<p><label>HTTP Method(s):</label>POST, DELETE</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200: Success. Returns the deleted PointDataComment object.</li>
				<li>404: Error.</li>
			</ul>
		</p>   
   </div>

   <div class="api_block">
		<div class="api_url">/Comments/Edit.{format}/{commentId}</div>
		<span class="api_description"></span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{commentId}. (unsigned integer)</li>
			</ul>
		</p>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>Text. (string)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>Minimum role required:</label>Moderator</p>
		<p><label>HTTP Method(s):</label>POST</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200: Success. Returns the updated PointDataComment object.</li>
				<li>404: Error.</li>
			</ul>
		</p> 
   </div>

  <div class="api_block">
     <div class="api_url">AggregatedComment object</div>
      <pre>
         {
            "Comment":
            { 
                "Id":149,
                "Text":"Where Sidney Crosby scored the winning goal in overtime to defeat the USA in the gold medal game.",
                "CreatedOn":"\/Date(1267769460000)\/",
                "CreatedById":4,
                "SummaryId":249
            },
            "CommentAuthor":"vanguide",
            "ServiceName":"Twitter"
        }
      </pre>
  </div>

   
 </div>
</asp:Content>