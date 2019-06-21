using System;

namespace Dnn.Authentication.Auth0.Helpers
{
    public class HttpAntiForgeryException : Exception
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