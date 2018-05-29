#region Usings

using System;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;

#endregion

namespace Dnn.Authentication.Auth0.Components
{
    public class Auth0Client : Auth0ClientBase
    {

        #region Constructors

        public Auth0Client(int portalId, AuthMode mode) 
            : base(portalId, mode, "Auth0")
        {
            var config = Auth0ConfigBase.GetConfig(Service, portalId);

            TokenEndpoint = new Uri($"https://{config.Auth0Domain}/oauth/token");
            AuthorizationEndpoint = new Uri($"https://{config.Auth0Domain}/authorize");
            UserEndpoint = new Uri($"https://{config.Auth0Domain}/userinfo");

            Scope = "openid profile email";

            AuthTokenName = "Auth0UserToken";
            LoadTokenCookie(String.Empty);
        }

        #endregion

        protected override TimeSpan GetExpiry(string responseText)
        {
            TimeSpan expiry = TimeSpan.MinValue;
            if (!string.IsNullOrEmpty(responseText))
            {
                var dictionary = Json.Deserialize<IDictionary<string, object>>(responseText);
                expiry = new TimeSpan(0, 0, Convert.ToInt32(dictionary["expires_in"]));
            }
            return expiry;
        }

        protected override string GetToken(string responseText)
        {
            string authToken = String.Empty;
            if (!string.IsNullOrEmpty(responseText))
            {
                var dictionary = Json.Deserialize<IDictionary<string, object>>(responseText);
                authToken = Convert.ToString(dictionary["access_token"]);
            }
            return authToken;
        }
    }
}