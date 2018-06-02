using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using Auth0UserInfo = Auth0.AuthenticationApi.Models.UserInfo;

namespace Dnn.Authentication.Auth0.Components
{
    public class Auth0Client
    {
        private const string _Scope = "openid profile email";
        private const string _OAuthCodeKey = "code";
        private const string _Service = "Auth0";
        //private const string _AuthTokenName = "Auth0UserToken";
        //private const string _OAuthTokenKey = "oauth_token";

        private AccessTokenResponse TokenResponse { get; set; }

        public string RedirectUrl { get; private set; }
        public string AuthToken { get; private set; }
        public Auth0ConfigBase Config { get; private set; }

        public string VerificationCode => HttpContext.Current.Request.Params[_OAuthCodeKey];

        public Auth0Client(int portalId)
        {
            Config = Auth0ConfigBase.GetConfig(portalId);

            RedirectUrl = Globals.LoginURL(String.Empty, false);
        }

        public bool IsCurrentService()
        {
            throw new NotImplementedException();
        }

        public bool HaveVerificationCode()
        {
            return !string.IsNullOrEmpty(VerificationCode);
        }

        public bool IsCurrentUserAuthorized()
        {
            return !string.IsNullOrEmpty(TokenResponse?.AccessToken);
        }

        public void Authorize()
        {
            //TODO: Verify that we have a valid domain defined
            var apiClient = new AuthenticationApiClient(Config.TenantDomain);

            var authBuilder = apiClient.BuildAuthorizationUrl()
                .WithResponseType(AuthorizationResponseType.Code)
                .WithClient(Config.ClientId)
                .WithRedirectUrl(RedirectUrl)
                .WithScope(_Scope);

            var authorizationUrl = string.IsNullOrEmpty(Config.ConnectionName) ? authBuilder.Build() : authBuilder.WithConnection(Config.ConnectionName).Build();

            HttpContext.Current.Response.Redirect(authorizationUrl.ToString(), true);

        }

        public async Task<AuthorisationResult> ExchangeCodeForTokenAsync()
        {
            var apiClient = new AuthenticationApiClient(Config.TenantDomain);

            try
            {
                TokenResponse = await apiClient.GetTokenAsync(new AuthorizationCodeTokenRequest()
                {
                    ClientId = Config.ClientId,
                    ClientSecret = Config.ClientSecret,
                    Code = VerificationCode,
                    RedirectUri = RedirectUrl
                });
            }
            catch (AggregateException ex)
            {
                // TODO: Handle token error
            }

            return (TokenResponse == null) ? AuthorisationResult.Denied : AuthorisationResult.Authorized;
        }

        public async Task<Auth0UserInfo> GetCurrentUserAsync()
        {
            if (!IsCurrentUserAuthorized())
                return null;

            var apiClient = new AuthenticationApiClient(Config.TenantDomain);
            var user = await apiClient.GetUserInfoAsync(TokenResponse.AccessToken);

            return user;
        }

        public virtual void AuthenticateDnnUser(Auth0UserInfo user, PortalSettings settings, string IPAddress, Action<UserAuthenticatedEventArgs> onAuthenticated)
        {
            var loginStatus = UserLoginStatus.LOGIN_FAILURE;

            var objUserInfo = UserController.ValidateUser(
                settings.PortalId, 
                user.UserId, 
                "",
                _Service, 
                "",
                settings.PortalName, 
                IPAddress,
                ref loginStatus);


            //Raise UserAuthenticated Event
            var eventArgs = new UserAuthenticatedEventArgs(objUserInfo, user.UserId, loginStatus, _Service)
            {
                AutoRegister = true
            };

            var profileProperties = new NameValueCollection();

            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.FirstName) && !string.IsNullOrEmpty(user.FirstName)))
            {
                profileProperties.Add("FirstName", user.FirstName);
            }
            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.LastName) && !string.IsNullOrEmpty(user.LastName)))
            {
                profileProperties.Add("LastName", user.LastName);
            }
            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.Email) && !string.IsNullOrEmpty(user.Email)))
            {
                profileProperties.Add("Email", user.Email);
            }
            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.DisplayName) && !string.IsNullOrEmpty(user.FullName)))
            {
                profileProperties.Add("DisplayName", user.FullName);
            }
            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.Profile.GetPropertyValue("ProfileImage")) && !string.IsNullOrEmpty(user.Picture)))
            {
                profileProperties.Add("ProfileImage", user.Picture);
            }
            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.Profile.GetPropertyValue("Website")) && !string.IsNullOrEmpty(user.Website)))
            {
                profileProperties.Add("Website", user.Website);
            }
            if ((objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.Profile.GetPropertyValue("PreferredLocale")))) && !string.IsNullOrEmpty(user.Locale))
            {
                if (LocaleController.IsValidCultureName(user.Locale.Replace('_', '-')))
                {
                    profileProperties.Add("PreferredLocale", user.Locale.Replace('_', '-'));
                }
                else
                {
                    profileProperties.Add("PreferredLocale", settings.CultureCode);
                }
            }

            if (objUserInfo == null || (string.IsNullOrEmpty(objUserInfo.Profile.GetPropertyValue("PreferredTimeZone"))))
            {
                    profileProperties.Add("PreferredTimeZone", user.ZoneInformation);
            }

            eventArgs.Profile = profileProperties;

            //SaveTokenCookie(String.Empty);

            onAuthenticated(eventArgs);
        }

    }
}