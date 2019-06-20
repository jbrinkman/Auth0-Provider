using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Dnn.Authentication.Auth0.Helpers
{
    public class AntiForgery
    {
        private const string AntiForgeryTokenName = "__AntiXsrfToken";
        private const string MissingStateTokenExceptionMessage = "Missing State Token";
        private const string MissingCookieTokenExceptionMessage = "Missing Cookie Token";
        private const string AntiForgeryTokenMismatchExceptionMessage = "AntiForgery tokens don't match";

        private static string _cookieName;

        public static string CookieName
        {
            get
            {
                if (AntiForgery._cookieName == null)
                    AntiForgery._cookieName = AntiForgery.GetAntiForgeryCookieName(HttpRuntime.AppDomainAppVirtualPath);
                return AntiForgery._cookieName;
            }
            set
            {
                AntiForgery._cookieName = value;
            }
        }

        internal static string GetAntiForgeryCookieName(string appPath)
        {
            if (string.IsNullOrEmpty(appPath) || appPath == "/")
                return AntiForgeryTokenName;
            return $"{AntiForgeryTokenName}_{HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(appPath))}";
        }

        public static HttpCookie GenerateCookie(bool IsSecure)
        {
            var token = Guid.NewGuid().ToString("N");
            var responseCookie = new HttpCookie(CookieName)
            {
                HttpOnly = true,
                Value = token
            };
            if (FormsAuthentication.RequireSSL && IsSecure)
            {
                responseCookie.Secure = true;
            };
            return responseCookie;
        }

        public static void ValidateTokens(HttpRequest Request)
        {
            var state = Request.QueryString["state"];
            var token = Request.Cookies[AntiForgeryTokenName]?.Value;

            if (string.IsNullOrEmpty(state)) throw new HttpAntiForgeryException(MissingStateTokenExceptionMessage);
            if (string.IsNullOrEmpty(token)) throw new HttpAntiForgeryException(MissingCookieTokenExceptionMessage);
            if (state != token) throw new HttpAntiForgeryException(AntiForgeryTokenMismatchExceptionMessage);
        }
    }
}