using Dnn.Authentication.Auth0.Components;
using Dnn.Authentication.Auth0.Helpers;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using static DotNetNuke.Services.Log.EventLog.EventLogController;
using Auth0UserInfo = Auth0.AuthenticationApi.Models.UserInfo;

namespace Dnn.Authentication.Auth0
{
    public partial class Login : AuthenticationLoginBase

    {
        public override bool Enabled => Auth0ConfigBase.GetConfig(PortalId).Enabled;

        protected virtual string AuthSystemApplicationName => "Auth0";

        protected Auth0Client Client { get; set; }

        public void AddEventLog(string username, int userId, string portalName, string ip, EventLogType logType, string message)
        {
            //initialize log record
            var objSecurity = new PortalSecurity();
            var log = new LogInfo
            {
                LogTypeKey = logType.ToString(),
                LogPortalID = PortalId,
                LogPortalName = portalName,
                LogUserName = objSecurity.InputFilter(username, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoAngleBrackets | PortalSecurity.FilterFlag.NoMarkup),
                LogUserID = userId
            };
            log.AddProperty("IP", ip);
            log.AddProperty("Message", message);

            //create log record
            var logctr = new LogController();
            logctr.AddLog(log);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            loginButton.Click += loginButton_Click;
            Client = new Auth0Client(PortalId);

            ClientResourceManager.RegisterStyleSheet(Page, "~/DesktopModules/AuthenticationServices/Auth0/authprovider.css");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Check to see if the login failed
            string errorReason = HttpContext.Current.Request.Params["error_reason"];
            if (errorReason != null)
            {
                Skin.AddModuleMessage(this, Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                AddEventLog("unknown user", Null.NullInteger, PortalSettings.PortalName, IPAddress, EventLogType.LOGIN_FAILURE, errorReason);

                return;
            }

            if (IsPostBack)
            {
                //Save the return Url in the cookie
                HttpContext.Current.Response.Cookies.Set(new HttpCookie("returnurl", RedirectURL)
                {
                    Expires = DateTime.Now.AddMinutes(5),
                    Path = (!string.IsNullOrEmpty(Globals.ApplicationPath) ? Globals.ApplicationPath : "/")
                });
            }

            Page.RegisterAsyncTask(new PageAsyncTask( GetUserAsync));
        }

        private async Task GetUserAsync()
        {
            if (Client.HaveVerificationCode())
            {
                try
                {
                    // Step 2: Validate that the state is the same as what was stored in the 
                    // CSRF cookie. Validation failures will throw exceptions.
                    AntiForgery.ValidateTokens(Request);

                    // Step 3: The app has redirected back with an Auth0 Code that can 
                    // be used for retrieving a Bearer Token from Auth0.
                    AuthorisationResult result = await Client.ExchangeCodeForTokenAsync();

                    switch (result)
                    {
                        case AuthorisationResult.Authorized:
                            // Step 4: We now have a token and can use that to get the
                            // user profile data from Auth0. The profile data we have 
                            // access to is controlled by the scope passed in the original
                            // authorization request.
                            Auth0UserInfo user = await Client.GetCurrentUserAsync();

                            // Step 5: We need to take the authenticated user profile and use it to 
                            // find the corresponding DNN User. If the user does not exist,
                            // we create the user and authorize the user according to the portal settings.
                            Client.AuthenticateDnnUser(user, PortalSettings, IPAddress, base.OnUserAuthenticated);
                            break;


                        case AuthorisationResult.Denied:
                            Skin.AddModuleMessage(this, Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                            AddEventLog("unknown user", Null.NullInteger, PortalSettings.PortalName, IPAddress, EventLogType.LOGIN_FAILURE, "Unable to get token");
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    (new ExceptionLogController()).AddLog(ex);
                }

            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // Step 1: Initiate Authorization Flow
            // This will redirect the browser to the Universal Login page.
            // Once the login is completed, the user will be redirected back
            // to the DNN Login page which will pick up processing in the OnLoad event
            Client.Authorize(Response, Request.IsSecureConnection);
        }
    }
}