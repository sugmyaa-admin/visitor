using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Text;

namespace Sufinn.Visitor.Services
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;

        public HangfireAuthorization(string username, string password)
        {
            _username = username;
            _password = password;
        }
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Check if Authorization header exists
            var header = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(header))
            {
                Challenge(httpContext);
                return false;
            }

            // Check if it’s Basic auth
            if (!header.ToString().StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                Challenge(httpContext);
                return false;
            }

            // Decode credentials
            var encodedUsernamePassword = header.ToString().Substring("Basic ".Length).Trim();
            var encoding = Encoding.UTF8;
            string decodedUsernamePassword;
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(encodedUsernamePassword);
                decodedUsernamePassword = encoding.GetString(base64EncodedBytes);
            }
            catch
            {
                Challenge(httpContext);
                return false;
            }
            var parts = decodedUsernamePassword.Split(':');
            if (parts.Length != 2)
            {
                Challenge(httpContext);
                return false;
            }

            var username = parts[0];
            var password = parts[1];

            if (username == _username && password == _password)
            {
                return true;
            }

            Challenge(httpContext);
            return false;
        }

        private void Challenge(HttpContext context)
        {
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        }
    }
}



