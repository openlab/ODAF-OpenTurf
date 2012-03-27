<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ODMasterPage2.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">
    <script type="text/javascript">
        var oauth_token = '<%=this.ViewData["oauth_token"] %>';
        var denied = '<%=this.ViewData["denied"]%>';

        function doCallback() {
            // Note: this does not work in IE
            if (window.opener != null) {

                try {
                    window.opener.callbackTW(oauth_token);
                }
                catch (e) {

                }
            }
            else {
                alert("window.opener is null!");
            }
        }

        function getParameter() {
            var path = window.location.href;
            var twOauth_token = path.substring(path.indexOf('oauth_token=') + 12);
            window.opener.callbackTW(twOauth_token);
        }


    </script>

    <style type="text/css">
        body
        {
            background:#40225F url(<%= ResolveUrl("~/images/background-tile-vertical.png")%>) repeat-y;
            font-family:Verdana;
            font-size:10pt;
            color:#FFF;
            width:100%;
            height:100%;
            
            vertical-align:middle;
        }
        
        .confirmed
        {
        	margin:10 auto;
        	border:none;
        	height:100%;
        	width:100%;
        	min-height:400px;
        	
        }

        .confirmed img
        {
        	float:left;
        	clear:none;
        	margin:0px 20px 0px 20px;
        }
        
        .closeBtn
        {
            background-image:url(<%= ResolveUrl("~/images/button.png") %>);
        	border:none;
        	width:150px;
        	height:25px;
        	color:#FFF;
        	text-align:center;
        	font-weight:bold;
        }
        
        .closeBtn:hover
        {
            background-image:url(<%= ResolveUrl("~/images/button_hover.png") %>);
        }
        
        .closeBtn:active
        {
            background-image:url(<%= ResolveUrl("~/images/button_pressed.png") %>);
        }
 
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BodyContent" runat="server">
<body onload="doCallback();">

   <div class="confirmed">
        
        <img style="position:absolute;z-index:1;left:18px;top:120px;" src='<%= ResolveUrl("~/images/logo.png") %>' alt="<%=website_mvc.Code.CloudSettingsResolver.GetConfigSetting("AppName") as string%>" />
        
        <table width="100%" style="padding-top:200px;">
            <tr>
                <td>
                    <img src="<%= ResolveUrl("~/images/TwitterSignin.png") %>" alt="Twitter" />
                </td>
                <td>
                    <div>
                        Cool, you signed in with Twitter.<br />
                        You can close this window and continue exploring.
                    </div>
                </td>
                <td>
                    <button class="closeBtn" onclick="window.close();">Close Window</button>
                </td>
            
            </tr>
            
        </table>
   </div>
</body>
</asp:Content>
