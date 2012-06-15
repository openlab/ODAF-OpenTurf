using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace website_mvc.Code
{
    public static class CloudSettingsResolver
    {
        private static bool inCloud = false;

        static CloudSettingsResolver()
        {
            inCloud = RoleEnvironment.IsAvailable;
        }

        public static string GetConnectionString(string name)
        {
            string connectionString = (inCloud)
                        ? RoleEnvironment.GetConfigurationSettingValue(name)
                        : ConfigurationManager.ConnectionStrings[name].ConnectionString;

            return connectionString;
        }

        public static string GetConfigSetting(string name)
        {
            string setting = (inCloud)
                        ? RoleEnvironment.GetConfigurationSettingValue(name)
                        : ConfigurationManager.AppSettings[name];

            return setting;
        }
    }
}
