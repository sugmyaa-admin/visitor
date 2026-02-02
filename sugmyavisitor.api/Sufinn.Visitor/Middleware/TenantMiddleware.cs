using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var host = context.Request.Host.Host;
            var parts = host.Split('.');

            if (parts.Length >= 3)
            {
                var tenant = parts[0]; // client1
                context.Items["Tenant"] = tenant;
            }

            await _next(context);
        }
    }

}
