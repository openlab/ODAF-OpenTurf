<%@ Page Language="C#"  MasterPageFile="~/Views/Shared/ODDocsMasterPage.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
ODAF Openturf REST API for User Accounts
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
<link href="<%= ResolveUrl("~/Content/api.css")%>" rel="stylesheet" type="text/css">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div>
   <h1>ODAF Openturf REST API for User Accounts (Twitter OAuth)</h1>

   <div class="api_block">
		<div class="api_url">/User/RequestAuthToken.{format}</div>
		<span class="api_description">Requests an OAuth token from Twitter, this is the first part of the authentication workflow (browser-based)</span>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Returns { oauth_token : "&lt;value&gt;", link: "&lt;authorization_url&gt;" }</li>
				<li>400. Error</li>
			</ul>
		</p>
   </div>
   
   <div class="api_block">
		<div class="api_url">/User/GetAccessToken.{format}/?oauth_token=</div>
		<span class="api_description">Gets an access OAuth token from Twitter, this is the second part of the authentication workflow (browser-based)</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>oauth_token. (string) This is the same OAuth token returned by /User/RequestAuthToken.</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Returns { oauth_token : "&lt;value&gt;", oauth_token_secret : "&lt;value&gt;" }</li>
				<li>400. Invalid/expired token</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">/User/Authenticate.{format}/?appId=&amp;oauth_token=&amp;oauth_token_secret=</div>
		<span class="api_description">Authenticates the user with Twitter using OAuth, and also with this web app.
		    This is the second part of the authentication workflow (browser-based). 
		    If using xAuth, you skip the RequestAuthToken and GetAccessToken steps, and you connect directly to Twitter using xAuth to get these tokens.</span>
		<p>
			<label>Parameters:</label>
			<ul>
			    <li>appId. (string) This is the appId of the app (admin registered on the DB)</li>
				<li>oauth_token. (string) This is the oauth_token returned from /User/GetAccessToken</li>
				<li>oauth_token_secret. (string) This is the oauth_token_secret returned from /User/GetAccessToken</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Success, saves the session as authenticated, returns the Twitter user object</li>
				<li>400. Expired token (our web-app session expiry, not Twitter's access token expiry)</li>
				<li>403. Forbidden (web-app banned user)</li>
				<li>401. Unauthorized (could not verify credentials with Twitter)</li>
			</ul>
		</p>
   </div>
           
   <div class="api_block">
		<div class="api_url">/User/Current.{format}</div>
		<span class="api_description">Gets the current user's User object</span>
		<p><label>Formats:</label>json, html</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Success, returns the current user's User object</li>
				<li>404. The current user is not found, most likely because you are not authenticated</li>
			</ul>
		</p>
   </div>
   
      <div class="api_block">
		<div class="api_url">/User/Info/{id}.{format}</div>
		<span class="api_description">returns the user's basic info</span>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Success, returns the current user's User object</li>
				<li>404. The current user is not found, most likely because you are not authenticated</li>
			</ul>
			
			<pre>
            {
                "Id":15,
                "screen_name":"VanGuide",
                "profile_image_url":"http://twitter.com/....."
            }
            </pre>
		</p>
   </div>
   
      <div class="api_block">
		<div class="api_url">/User/Image/{id}.{format}</div>
		<span class="api_description">Redirects to the user's twitter profile image url</span>
		<p><label>Requires authentication:</label>false</p>
		<p><label>HTTP Method(s):</label>GET</p>

   </div>

   <div class="api_block">
		<div class="api_url">/User/UpdateTwitterStatus.{format}/?status=&amp;lat=&amp;lng=</div>
		<span class="api_description">Updates the current user's Twitter status, with optional lat and long coordinates.</span>
		<p>
			<label>Parameters:</label>
			<ul>
				<li>status. (string) the status to post to Twitter</li>
				<li>lat. (float, optional) latitude for the tweet</li>
				<li>lng. (float, optional) longitude for the tweet</li>
			</ul>
		</p>
		<p><label>Formats:</label>json</p>
		<p><label>Requires authentication:</label>true</p>
		<p><label>HTTP Method(s):</label>POST</p>
		<p>
			<label>Response:</label>
			<ul>
				<li>200. Successfully updated the Twitter status</li>
				<li>400. Error</li>
			</ul>
		</p>
   </div>

   <div class="api_block">
		<div class="api_url">Authentication Workflow</div>
        <ol>
            <li>Get request_token from web service (User/RequestAuthToken)</li>
            <li>Pass this request_token to Twitter's authorization page as an oauth_token query variable</li>
            <li>At the end of (2)'s process, we then call User/GetAccessToken above, passing in the same value from (1), save the oauth_token and oauth_token_secret return values</li>
            <li>From the two values in (3) we call (User/Authenticate) above - if successful, we are authenticated and can call the other API functions</li>
        </ol>
   </div>

   <div class="api_block">
		<div class="api_url">API Notes</div>
        <p>
            Most of the web service APIs will require authentication. API functions that require super privileges will be marked as such (Admin users). 
            Setting of a user to be "Administrator" or "Moderator" can only be done currently by a T-SQL command direct at the database.
        </p>
   </div>

   <div class="api_block">
		<div class="api_url">Security Notes</div>
        <p>
            /User/Authenticate -- saving the oauth_token and oauth_token_secret has the same security concerns as saving a username and password. 
            If an attacker somehow intercepts this data, they can re-use (replay) these tokens. We set up our own "TokenExpiry", and force users to re-authenticate going through the RequestToken/GetAccessToken flow again (to reset TokenExpiry).
            Also, users can revoke the access for the Twitter application, so as to expire these tokens.
        </p>
   </div>
        
   <div class="api_block">
		<div class="api_url">User Object</div>
        <pre>
            {
                "Id":15,
                "user_id":118340338,
                "screen_name":"VanGuide",
                "oauth_token":"1183wK40338-eCUPv70lOgkbHUvqUoy15dtf6TPY5lGfBNtQTUIyoDw",
                "oauth_token_secret":"STBbWSs3bMTgeI4y5KfMnLofHu4bH8LwVjlzOOuuOyxXIDY",
                "UserRole":0,
                "UserAccess":0,
                "LastAccessedOn":"\/Date(1268892600000)\/",
                "TokenExpiry":"\/Date(1269497400000)\/",
                "oauth_service_id":2,
                "profile_image_url":"http://a1.twimg.com/profile_images/723248386/icon_normal.png"
            } 
            
            Values are cumulative:
            UserRole - 0 (Member), 1 (Moderator), 2 (Administrator)
            UserAccess - 0 (Normal), 1 (Pending), 2 (Deleted), 3 (Banned)  
            
            Date values are based on seconds since Unix time (seconds since Jan 1st, 1970 0:00 GMT) 
            oauth_service_id is the Id of the OAuthClientApp record
            user_id is the user id from the oauth service (in this case Twitter)   
        </pre>
   </div>


</div>
</asp:Content>
