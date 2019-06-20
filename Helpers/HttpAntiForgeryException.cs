using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dnn.Authentication.Auth0.Helpers
{
    public class HttpAntiForgeryException: Exception
    {
        public HttpAntiForgeryException()
        {
        }

        public HttpAntiForgeryException(string message)
            : base(message)
        {
        }

        public HttpAntiForgeryException(string message, Exception inner)
            : base(message, inner)
        {
        }


    }
}