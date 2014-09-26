using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;

[assembly: OwinStartup(typeof(ODAF.Website.Startup))]
namespace ODAF.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}