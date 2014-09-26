using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin.Security;
using System.IdentityModel.Tokens;
using Owin;
using System.Linq;
using Microsoft.Owin.Security.Twitter;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Configuration;
using System.IdentityModel.Services;
using System.Web;
using ODAF.Website.Helpers;

namespace ODAF.Website
{
    public partial class Startup
    {

        public void ConfigureAuth(IAppBuilder app)
        {

            app.SetDefaultSignInAsAuthenticationType(WsFederationAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = UserAccountType.External,
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive,
            }); 
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = UserAccountType.Twitter,
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
            });

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = WsFederationAuthenticationDefaults.AuthenticationType,
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active
                });



            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    MetadataAddress = ConfigurationManager.AppSettings["FederationMetadataAddress"],
                    Wtrealm = ConfigurationManager.AppSettings["FederationWtrealm"],
                    SignInAsAuthenticationType = WsFederationAuthenticationDefaults.AuthenticationType
                });

            app.UseTwitterAuthentication(
                new TwitterAuthenticationOptions
               {
                   ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"],
                   ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                   Provider = new Microsoft.Owin.Security.Twitter.TwitterAuthenticationProvider
                   {
                       OnAuthenticated = async context =>
                       {
                           context.Identity.AddClaim(new Claim("urn:twitter:accesstoken", context.AccessToken));
                           context.Identity.AddClaim(new Claim("urn:twitter:accesstokensecret", context.AccessTokenSecret));
                       }
                   },
                   SignInAsAuthenticationType = UserAccountType.External
               });

        }
    }
}