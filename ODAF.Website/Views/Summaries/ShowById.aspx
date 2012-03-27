<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ODMasterPage2.Master" CodeBehind="ShowById.aspx.cs" Inherits="Views_Summaries_ShowById" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="vancouveropendata.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
<%=website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string%> Location - <%=ViewData.Model.Name %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
    <meta name="viewport" content="width = device-width, initial-scale = 1.0, maximum-scale = 1.0, user-scalable = 0" />
    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    <script type="text/javascript">

        var Vanc = new VELatLong(49.241810639098816, -123.06404113769532);
        var map = null;
        
        var id = "<%=ViewData.Model.Guid %>";
        var title = "<%=ViewData.Model.Name %>";
        var description = "<%=ViewData.Model.Description%>";
        var lat = <%=ViewData.Model.Latitude %>;
        var lng = <%=ViewData.Model.Longitude %>;
        var icon = '<%= ResolveUrl("~/images/pin.png") %>';
        var pointToShow = null;
        var coords = new VELatLong(lat, lng);
        <% if (Request.Browser.IsMobileDevice) { %>
            var deviceWidth = <%=Request.Browser.ScreenPixelsWidth%>;
            var deviceHeight = <%=Request.Browser.ScreenPixelsHeight%>/2;
        <% } else { %>
            var deviceWidth = 640;
            var deviceHeight = 480;
        <% } %>
       
        document.body.addEventListener("load", function() { 
            setTimeout(hideURLbar, 0); 
            }, false); 

        function hideURLbar() { 
            window.scrollTo(0, 1); 
        } 
           
        function UpdateOrientation()
        {
            switch(window.orientation)
            {
                case 0: // portrait
                case 180: // portrait
                    map.Resize(deviceWidth, deviceHeight);
                    break;
                case -90: // landscape
                case 90: // landscape
                    map.Resize(deviceHeight*2, deviceWidth/2);
                    break;
            }    
        }       
                
        function GetMap() {
            map = new VEMap("BingMap");
            map.LoadMap(Vanc, 10, VEMapStyle.Aerial, false, VEMapMode.Mode2D, true, 1);
            map.SetCredentials("AiN8LzMeybPbj9CSsLqgdeCG86jg08SJsjm7pms3UNtNTe8YJHINtYVxGO5l4jBj");
            
            pointToShow = new VEPushpin(id, coords, icon, title, description);
            map.AddPushpin(pointToShow);
            map.Resize(deviceWidth, deviceHeight);
            map.SetCenter(coords);
       }

    </script>
    <style type="text/css">
          
      body
      {
      	 background-color:#333;
      	 margin: 0;
      	 padding: 0;
      	 font-family:Trebuchet MS;
      	 color:#ddd;
      	 width: 100%;
      }
      
      h1 {
        margin-left: 5px;
        font-size: 1em;
      }
      
      h2 {
        margin-left: 5px;
        font-size: 0.7em;
      }
      
      .data_label {
        font-weight: bold;
        font-size: 1.0em;
        width: 20px;
      }
      
      #data_attributes {
          margin: 10px 0 10px 5px;
      }
      
      .data {
        font-size: 0.9em;
      }
      
      .truncate
      {
          text-overflow: ellipsis;
          overflow: hidden;
          width: 100%;
          white-space: nowrap;
      }
      
      .data a 
      {
          text-decoration: none;
          color: blue;
      }
      
      #BingMap 
      {
        position:relative;
      }
      
      </style>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" runat="server">

<body onload="GetMap();" onorientationchange="UpdateOrientation();">
    <div>
        <h1 class="truncate"><%=website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string%> - <%=ViewData.Model.Name %></h1>
        <h2 class="truncate"><%=ViewData.Model.Description %></h2>
    </div>
    <div id="BingMap">
    </div>
    <div id="data_attributes">
    <div class="pair truncate">
        <span class="data_label">
            Rating:
        </span>
        <span class="data">
            <%
                var rating = 0.0;
                var rating_text = ViewData.Model.RatingCount == 1 ? "rating" : "ratings";
                if (ViewData.Model.RatingCount > 0)
                {
                    rating = ((double)(ViewData.Model.RatingTotal / ViewData.Model.RatingCount) * 5.0) / 100.0;
                }
            %>
            <%=rating %>/5 stars (on <%=ViewData.Model.RatingCount %> <%=rating_text %>)
        </span>
    </div>
    <div class="pair truncate">
        <span class="data_label">
            Tags:
        </span>
        <span class="data">
            <%
                var tag_text = ViewData.Model.Tag.Length == 0 ? "none." : ViewData.Model.Tag;
            %>
            <%=tag_text %>
        </span>
    </div>
    <div class="pair truncate">
        <span class="data_label">
            Comments:
        </span>
        <span class="data">
            <%
                var comment_text = ViewData.Model.CommentCount == 1 ? "comment" : "comments";
                comment_text = String.Format("{0} {1}", ViewData.Model.CommentCount, comment_text);
            %>
            <% if (Request.Browser.IsMobileDevice && Request.Browser.Platform.Equals("iPhone OS"))
               { %>
                <a id="app-link" href="<%=(website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string).ToLower()%>://ViewSummary/?<%=String.Format("SummaryId={0}", ViewData.Model.Id) %>"><%=comment_text%></a>
            <% } else { %>
               <%=comment_text%>
            <%} %>
        </span>
    </div>
    </div>
</body>

</asp:Content>

