using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IdentityModel.Tokens;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Web.Administration;

namespace ODAF.Website
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // See the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            RoleEnvironment.Changing += RoleEnvironmentChanging;

            //Comment following code to debug the cloud service on local emulator

            #region Copy the config in web.config
            using (var server = new ServerManager())
            {
                string siteNameFromServiceModel = "Web";
                string siteName = string.Format("{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, siteNameFromServiceModel);
                string configFilePath = server.Sites[siteName].Applications[0].VirtualDirectories[0].PhysicalPath + "\\Web.config";
                XElement element = XElement.Load(configFilePath);

                string strSetting;

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("DBConnectionString"))))
                {
                    var v = from appSetting in element.Element("connectionStrings").Elements("add")
                            where "ODAF" == appSetting.Attribute("name").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("connectionString").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("BingCredential"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "BingCredential" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("TwitterConsumerKey"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "TwitterConsumerKey" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("TwitterConsumerSecret"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "TwitterConsumerSecret" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("FederationMetadataAddress"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "FederationMetadataAddress" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                if (!(String.IsNullOrEmpty(strSetting = RoleEnvironment.GetConfigurationSettingValue("FederationWtrealm"))))
                {
                    var v = from appSetting in element.Element("appSettings").Elements("add")
                            where "FederationWtrealm" == appSetting.Attribute("key").Value
                            select appSetting;

                    if (v != null) v.First().Attribute("value").Value = strSetting;
                }

                element.Save(configFilePath);

            }
            #endregion



            return base.OnStart();
        }
        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            e.Cancel = false;
        }

    }
}