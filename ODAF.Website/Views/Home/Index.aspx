<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/ODMasterPage.Master" Inherits="Views_Home_Index" Codebehind="Index.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
<%=ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OtherHeadContent" runat="server">

    <style type="text/css">
    html, body, iframe {
	    height: 100%;
	    overflow: auto;
	    
	    padding: 0;
	    margin: 0;
	    
    }
    body {
	    padding: 0;
	    margin: 0;
	    background-color:#555;

    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
	    padding: 0;
	    margin: 0;
    }
    #form1, #aspnetForm
    {
        height:100%;
        padding: 0;
	    margin: 0;
    }
    
    #msodSLapp
    {
        min-height:700px;	
        min-width:700px;
        padding: 0;
	    margin: 0;
    }
    </style>
    
    <script type="text/javascript" src="<%= ResolveUrl("~/Silverlight.js")%>"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

        function connectTW(url) {
            window.open(url, "SelectorWindow", "status=1, height=500, width=780, resizable=0");
        }

        // IE has a problem with this ???
        function callbackTW(oauth_token, oauth_verifier) {
            var plugin = document.getElementById('msodSLapp');
            plugin.Content.Page.OnJSTwitterCallback(oauth_token, oauth_verifier);
        }

    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server">
        <div id="SilverlightContent" style="display: none;"><%= ViewData["AppName"] %></div>
        <div id="silverlightControlHost">
            <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" 
                width="100%" height="100%" id="msodSLapp">
                <param name="source" value="<%= ResolveUrl("~/ClientBin/ODAF.SilverlightApp.xap") %>"/>
                <param name="onError" value="onSilverlightError" />
                <param name="enableGPUAcceleration" value="true" />
                <param name="background" value="#FFFFFFFF" />
                <param name="minRuntimeVersion" value="3.0.40624.0" />
                <param name="autoUpgrade" value="true" />
                <param name="enableAutoZoom" value="true" />
                <param name="enableCacheVisualization" value="false" />
                <param name="enableFramerateCounter" value="false" />
                <param name="enableRedrawRegions" value="false" />
                <param name="windowless" value="false" />
                
                <param name="initParams" value='appName=<%= ViewData["AppName"] %>,twitterAppId=<%= ViewData["TwitterAppId"] %>,pointDataUrl=PointSources.xml,regionDataUrl=RegionSources.xml,PointDataSummaryId=<%=ViewData["PointDataSummaryId"]%>' />
                <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40624.0" style="text-decoration:none">
                  <img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style:none"/>
                </a>
	        </object>
	        <iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>
        </div>
    </form>
</asp:Content>



