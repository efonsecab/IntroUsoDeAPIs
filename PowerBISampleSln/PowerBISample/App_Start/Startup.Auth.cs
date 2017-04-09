using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PowerBISample
{
    public class GlobalConstants
    {
        public const string PowerBIAccessToken = "PowerBIAccessToken";
    }
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string authority = aadInstance + tenantId;
        public static string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static string powerBIResourceUri = ConfigurationManager.AppSettings["ida:PowerBiResourceUri"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        AuthorizationCodeReceived = async (context) =>
                        {
                            string powerBiAccessToken = await AccessToken(clientId, clientSecret, context);
                            HttpContext.Current.Session[GlobalConstants.PowerBIAccessToken] = powerBiAccessToken;
                        }
                    },
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri
                });
        }

        private async Task<string> AccessToken(string clientId, string clientSecret, AuthorizationCodeReceivedNotification context)
        {
            var request = context.Request;
            var absoluteUrl = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), request.Path.ToUriComponent());
            string code = context.Code;
            ClientCredential credential = new ClientCredential(clientId, clientSecret);
            AuthenticationContext authContext = new AuthenticationContext(authority);
            AuthenticationResult powerBIAuthResult = await authContext.AcquireTokenByAuthorizationCodeAsync(code,
                new Uri(absoluteUrl), credential, powerBIResourceUri);
            return powerBIAuthResult.AccessToken;
        }
    }
}
