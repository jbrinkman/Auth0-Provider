#region Usings

using System.Collections.Generic;
using System.Text;

#endregion

namespace Dnn.Authentication.Auth0.Components
{
    internal static class AuthExtensions
    {
        public static string ToAuthorizationString(this IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("OAuth ");

            for (int i = 0; i < parameters.Count; i++)
            {
                string format = "{0}=\"{1}\"";

                QueryParameter p = parameters[i];
                sb.AppendFormat(format, Auth0ClientBase.UrlEncode(p.Name), Auth0ClientBase.UrlEncode(p.Value));

                if (i < parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        public static string ToNormalizedString(this IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }
    }

}