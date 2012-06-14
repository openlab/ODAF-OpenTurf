﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace ODAF.SilverlightApp
{
    public partial class App : Application
    {
        public string pointDataUrl = "/";
        public string regionDataUrl = "/";
        public string pageRootUrl = "/";
        public string pointDataSummaryId = "";
        public string appName = "OpenTurf";
        public string twitterAppId = "";
        public string GeoCodeServiceCredentials = "AiN8LzMeybPbj9CSsLqgdeCG86jg08SJsjm7pms3UNtNTe8YJHINtYVxGO5l4jBj"; 
        public App()
        {
            this.Startup += this.Application_Startup;
            
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Uri pageURI = System.Windows.Browser.HtmlPage.Document.DocumentUri;
            string portString = (pageURI.Port != 80) ? ( ":" + pageURI.Port ) : "";

            pageRootUrl = pageURI.Scheme + "://" + pageURI.Host + portString + "/";// +(pageURI.LocalPath.Substring(0, pageURI.LocalPath.LastIndexOf("/") + 1));

            if (e.InitParams.ContainsKey("pointDataUrl"))
            {
                pointDataUrl = e.InitParams["pointDataUrl"];
            }
            if (e.InitParams.ContainsKey("regionDataUrl"))
            {
                regionDataUrl = e.InitParams["regionDataUrl"];
            }
            if (e.InitParams.ContainsKey("PointDataSummaryId"))
            {
                pointDataSummaryId = e.InitParams["PointDataSummaryId"];
            }
            if (e.InitParams.ContainsKey("appName"))
            {
                appName = e.InitParams["appName"];
            }
            if (e.InitParams.ContainsKey("twitterAppId"))
            {
                twitterAppId = e.InitParams["twitterAppId"];
            }

            this.RootVisual = new MainPage();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
