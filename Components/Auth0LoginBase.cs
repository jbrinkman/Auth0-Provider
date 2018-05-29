#region Usings

using System;
using System.Collections.Specialized;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;

#endregion

namespace Dnn.Authentication.Auth0.Components
{
    public abstract class Auth0LoginBase : OAuthLoginBase
    {
        protected Auth0ClientBase Auth0Client { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                //Save the return Url in the cookie
                HttpContext.Current.Response.Cookies.Set(new HttpCookie("returnurl", RedirectURL)
                {
                    Expires = DateTime.Now.AddMinutes(5),
                    Path = (!string.IsNullOrEmpty(Globals.ApplicationPath) ? Globals.ApplicationPath : "/")
                });
            }

            bool shouldAuthorize = Auth0Client.IsCurrentService() && Auth0Client.HaveVerificationCode();
            if(Mode == AuthMode.Login)
            {
                shouldAuthorize = shouldAuthorize || Auth0Client.IsCurrentUserAuthorized();
            }

            if (shouldAuthorize)
            {
                if (Auth0Client.Authorize() == AuthorisationResult.Authorized)
                {
                    Auth0Client.AuthenticateUser(GetCurrentUser(), PortalSettings, IPAddress, AddCustomProperties, OnUserAuthenticated);
                }
            }
        }

        #region Overrides of AuthenticationLoginBase

        public override bool Enabled
        {
            get { return Auth0ConfigBase.GetConfig(AuthSystemApplicationName, PortalId).Enabled; }
        }

        #endregion
    }
}