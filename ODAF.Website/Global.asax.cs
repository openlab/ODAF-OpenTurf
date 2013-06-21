using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using vancouveropendata;
using System.Configuration;
using System.Reflection;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Globalization;
using System.Threading;

namespace website_mvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "DefaultFeedIndex",                                              // Route name
                "Feeds",                                                // URL with parameters
                new { controller = "Feeds", action = "Index", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "MobileOverrideFeedList",                                              // Route name
                "PointSources.xml",                                                // URL with parameters
                new { controller = "Feeds", action = "MobileFeeds", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "MobileFeedList",                                              // Route name
                "Feeds/MobileFeeds",                                                // URL with parameters
                new { controller = "Feeds", action = "MobileFeeds", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "MobileOverrideregionsList",                                              // Route name
                "RegionSources.{format}",                                                // URL with parameters
                new { controller = "Feeds", action = "RegionsMobile", format = "xml" }  // Parameter defaults
            );

            routes.MapRoute(
                "MobileRegionsList",                                              // Route name
                "Feeds/RegionsMobile/{format}",                                                // URL with parameters
                new { controller = "Feeds", action = "RegionsMobile", format = "xml" }  // Parameter defaults
            );

            routes.MapRoute(
                "MobileRegionsList2",                                              // Route name
                "Feeds/RegionsMobile.{format}",                                                // URL with parameters
                new { controller = "Feeds", action = "RegionsMobile", format = "xml" }  // Parameter defaults
            );

            routes.MapRoute(
                "DefaultFeedList2",                                              // Route name
                "Feeds/List",                                                // URL with parameters
                new { controller = "Feeds", action = "List", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "DefaultFeedProperList",                                              // Route name
                "Feeds/ProperList",                                                // URL with parameters
                new { controller = "Feeds", action = "ProperList", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "DefaultFeed",                                              // Route name
                "Feeds/{id}/{type}",                           // URL with parameters
                new { controller = "Feeds", action = "Get", id = "", type = "point" }  // Parameter defaults
            );

            routes.MapRoute(
                "DefaultWithFormat",                                              // Route name
                "{controller}/{action}.{format}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "", format = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

            ModelBinders.Binders.Add(typeof(string), new NullStringModelBinder());

        }

        protected void Application_Start()
        {
            hackAConnectionString("ODAF");
            RegisterRoutes(RouteTable.Routes);
        }

        //
        // Bit of an explanation needed here.  It seems that SubSonic (the data layer) will only
        //  ever look at the config specified in Web.config, however we need to grab this info from
        //  the service config, so we'll do a little voodoo and modify the ConfigurationManager.ConnectionStrings
        //  via reflection to do our bidding if it's running on Azure.
        private void hackAConnectionString(string name)
        {

            //Check that we are actually running in the cloud (or cloud emulator)
            if (RoleEnvironment.IsAvailable)
            {
                var realConnectionString = RoleEnvironment.GetConfigurationSettingValue(name);

                // Connection strings are actuall read only.  We'll fix 'em good.
                var settings = ConfigurationManager.ConnectionStrings[name];
                if (settings != null)
                {
                    var fi = typeof(ConfigurationElement).GetField("_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                    fi.SetValue(settings, false);

                    // Now set the real connection string from the service config.
                    settings.ConnectionString = realConnectionString;
                }
            }
        }
    }
}