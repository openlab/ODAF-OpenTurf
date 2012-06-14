<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ODDocsMasterPage.Master" Inherits="Views_Summaries_Index" Codebehind="Index.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Openturf REST API for Summaries
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
<link href="<%= ResolveUrl("~/Content/api.css")%>" rel="stylesheet" type="text/css">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div>
   <h1>ODAF Openturf REST API for Summaries</h1>

   <div class="api_block">
		<div class="api_url">/Summaries/ShowLayersByUserId.{format}/{userId}</div>
		<span class="api_description">Returns an array of GUIDs for the layers that have been created by the user.</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{userId}. (unsigned integer). If not set, will use the current user.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>An array of strings (GUIDS).</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Summaries/ShowByLayerId.{format}/?layerId=</div>
		<span class="api_description">Returns an array of PointDataSummary objects that have the layerId</span>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>layerId. string (GUID)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>An array of PointDataSummary objects.</li>
			</ul>
		</p>   
   </div>

   <div class="api_block">
		<div class="api_url">/Summaries/ShowByUserId.{format}/{userId}</div>
		<span class="api_description">Returns an array of PointDataSummary objects that have been created by the user with the userId</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{userId}. unsigned integer</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>An array of PointDataSummary objects.</li>
			</ul>
		</p>
   </div>
   
   <div class="api_block">
		<div class="api_url">/Summaries/ShowById.{format}/{summaryId}</div>
		<span class="api_description">Returns a PointDataSummary object with the summary id.</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>{summaryId}. unsigned integer</li>
			</ul>
		</p>
		<p><label>Formats:</label>json, html</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. json - Returns the PointDataSummary object, html - displays the object in a html page.</li>
				<li>404. Object not found.</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Summaries/ShowByGuid.{format}/?guid=</div>
		<span class="api_description">Returns a PointDataSummary object that has the GUID.</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>guid. string (GUID)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Returns the PointDataSummary object.</li>
				<li>404. Object not found.</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Summaries/Show.{format}/?lat=&lng=&layerId=</div>
		<span class="api_description">Returns a PointDataSummary object, that is at the latitude, longitude and has the layerId</span>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>lat. (float)</li>
				<li>lng. (float)</li>
				<li>layerId. string (GUID)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Returns the PointDataSummary object.</li>
				<li>404. Object not found.</li>
			</ul>
		</p>
  </div>

   <div class="api_block">
		<div class="api_url">Summaries/List.{format}/?page=&page_size=</div>
		<span class="api_description">Returns an array of PointDataSummary objects, with the newest objects first.</span>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>page. (integer, optional) First page is 0, and default value is 0.</li>
				<li>page_size. (integer, optional). Max page size and default value is <%=ViewData.Model["MAX_ITEMS_PER_PAGE"] %>.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>/
			<label>Response:</label>
			<ul>
				<li>Returns a list of PointDataSummary objects</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/Summaries/ListByRegion.{format}/?lat=&lng=&latDelta=&lngDelta=&layerId=&page=&page_size=</div>
		<span class="api_description">Returns a PointDataSummary object, that is within the region specified at center point at the latitude, longitude (with the deltas specified) and has the layerId</span>
		<p>
			<label>Query variable(s):</label>
			<ul>
				<li>lat. (float)</li>
				<li>lng. (float)</li>
				<li>latDelta. (float)</li>
				<li>lngDelta. (float)</li>
				<li>layerId. string (GUID)</li>
				<li>page. (integer, optional) First page is 0, and default value is 0.</li>
				<li>page_size. (integer, optional). Max page size and default value is <%=ViewData.Model["MAX_ITEMS_PER_PAGE"] %>.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>Returns a list of PointDataSummary objects</li>
			</ul>
		</p>
  </div>
   
		
   <div class="api_block">
		<div class="api_url">/Summaries/Add.{format}/</div>
		<span class="api_description">Adds a new PointDataSummary object.</span>
		<p>
			<label>Parameters(s):</label>
			<ul>
				<li>Name. (string)</li>
				<li>Description. (string)</li>
				<li>LayerId. (string, GUID)</li>
				<li>Latitude. (float)</li>
				<li>Longitude. (float)</li>
				<li>Tag. string</li>
				<li>Guid. (string)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>HTTP Method(s):</label>POST, PUT</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>201: Success. Returns the newly created PointDataSummary object. Also adds a Location: header with the URL of the newly created resource.</li>
				<li>400: Error.</li>
			</ul>
		</p>  
  </div>

  <div class="api_block">
		<div class="api_url">/Summaries/Search/</div>
		<span class="api_description">Searches PointDataSummary objects.</span>
		<p>
			<label>Parameters(s):</label>
			<ul>
				<li>layers. (string)</li>
				<li>id. (string)</li>
				<li>term. (string)</li>
				<li>tag. (string)</li>
				<li>name. (string)</li>
				<li>results. (integer default=100)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>201: Success. Returns a list of PointDataSummary objects.</li>
				<li>400: Error.</li>
			</ul>
		</p>  
  </div>
   
   <div class="api_block">
		<div class="api_url">/Summaries/Edit.{format}/{summaryId}</div>
		<span class="api_description">Edits a PointDataSummary object.</span>
		<p>
		<p>modify any of these properties "Description", "LayerId", "Latitude", "Longitude", "Tag", "Guid"</p>
			<label>Parameters:</label>
			<ul>
				<li>summaryId. (unsigned integer). The id of the PointDataSummary to edit.</li>
				<li>Name. (string)</li>
				<li>Description. (string)</li>
				<li>LayerId. (string, GUID)</li>
				<li>Latitude. (float)</li>
				<li>Longitude. (float)</li>
				<li>Tag. string</li>
				<li>Guid. (string)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>Minimum role required:</label>Moderator</p>
		<p><label>HTTP Method(s):</label>POST, PUT</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200: Success. Returns the edited PointDataSummary object.</li>
				<li>404: Error.</li>
			</ul>
		</p>   
  </div>

  <div class="api_block">
		<div class="api_url">/Summaries/Remove.{format}/{summaryId}</div>
		<span class="api_description">Removes a PointDataSummary object</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>summaryId. (unsigned integer). The id of the PointDataSummary to remove.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>Minimum role required:</label>Administrator</p>
		<p><label>HTTP Method(s):</label>POST, DELETE</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200: Success. Returns the deleted PointDataSummary object.</li>
				<li>404: Error.</li>
			</ul>
		</p>   
  </div>

   <div class="api_block">
		<div class="api_url">/Summaries/AddRating.{format}/{summaryId}/?rating=</div>
		<span class="api_description">Adds a rating to a PointDataSummary</span>
		<p>
			<label>Parameters(s):</label>
			<ul>
				<li>summaryId. (unsigned integer) the id of the PointDataSummary object to add the rating to.</li>
				<li>rating. (unsigned integer). the rating to add, from 0 to 100.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>HTTP Method(s):</label>POST</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200: Success. Returns the updated PointDataSummary object.</li>
				<li>400: Error.</li>
				<li>404: PointDataSummary not found.</li>
			</ul>
		</p>  
  </div>

   <div class="api_block">
		<div class="api_url">/Summaries/AddTag.{format}/{summaryId}/?tag=</div>
		<span class="api_description">Adds a tag to a PointDataSummary</span>
		<p>
			<label>Parameters(s):</label>
			<ul>
				<li>summaryId. (unsigned integer) the id of the PointDataSummary object to add the rating to.</li>
				<li>tag. (string). the tag to add (duplicates are not added)</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>HTTP Method(s):</label>POST</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200: Success. Returns the updated PointDataSummary object.</li>
				<li>400: Error.</li>
				<li>404: PointDataSummary not found.</li>
			</ul>
		</p>  
  </div>
  
   <div class="api_block">
		<div class="api_url">/Summaries/ShowForCommunityByActivity.{format}/?page=&page_size=</div>
		<span class="api_description">Returns an array of PointDataSummary objects created by the community, newest first (only <%=ViewData.Model["MAX_ITEMS_PER_PAGE"] %> returned).</span>
		<p>
			<label>Parameters(s):</label>
			<ul>
				<li>page. (integer, optional) First page is 0, and default value is 0.</li>
				<li>page_size. (integer, optional). Max page size and default value is <%=ViewData.Model["MAX_ITEMS_PER_PAGE"] %>.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label> an array of PointDataSummary objects
		</p>  
  </div>
  
	<div class="api_block">
	 <div class="api_url">PointDataSummary object</div>
	  <pre>
		{
			"Id":249,
			"Description":"Where Olympic Hockey was held for the Vancouver 2010 Winter Olympics",
			"Latitude":49.27766,
			"Longitude":-123.10869,
			"LayerId":"14522927",
			"Tag":"",
			"RatingCount":2,
			"RatingTotal":200,
			"CommentCount":1,
			"CreatedOn":"\/Date(1267769400000)\/",
			"ModifiedOn":"\/Date(1267844820000)\/",
			"Guid":"7faeabf5-340d-9c07-3902-690af3ca90d1",
			"Name":"GM Place / Canada Hockey Place",
			"CreatedById":4
	   }
	  </pre>
  </div>
		
</div>
</asp:Content>