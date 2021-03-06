using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System.Web;
using System.Web.Mvc;

namespace Corno.Raychem.CustomerPortal
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        private readonly UrlHelper _url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(_url.Action("Login", "Account", new { area = "Admin", controller = "Account" }))
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            app.UseGoogleAuthentication(
         clientId: "97953975135-ho9pkaqbvu8il0e8q9c66fma71qlgpp3.apps.googleusercontent.com",
         clientSecret: "OoUWjgplBtCX9jI6nDrRX-EY");
        }
    }

    //public partial class Startup
    //{
    //    // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
    //    private readonly UrlHelper _url = new UrlHelper(HttpContext.Current.Request.RequestContext);

    //    public void ConfigureAuth(IAppBuilder app)
    //    {
    //        // Enable the application to use a cookie to store information for the signed in user
    //        app.UseCookieAuthentication(new CookieAuthenticationOptions
    //        {
    //            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
    //            //LoginPath = new PathString("/Account/Login")
    //            LoginPath =
    //                new PathString(_url.Action("Login", "Account", new {area = "Admin", controller = "Account"}))
    //        });
    //        // Use a cookie to temporarily store information about a user logging in with a third party login provider
    //        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

    //        // Uncomment the following lines to enable logging in with third party login providers
    //        //app.UseMicrosoftAccountAuthentication(
    //        //    clientId: "",
    //        //    clientSecret: "");

    //        //app.UseTwitterAuthentication(
    //        //   consumerKey: "",
    //        //   consumerSecret: "");

    //        //app.UseFacebookAuthentication(
    //        //   appId: "",
    //        //   appSecret: "");

    //        app.UseGoogleAuthentication(
    //            "97953975135-ho9pkaqbvu8il0e8q9c66fma71qlgpp3.apps.googleusercontent.com",
    //            "OoUWjgplBtCX9jI6nDrRX-EY");
    //    }
    //}
}