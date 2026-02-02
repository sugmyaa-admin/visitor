using Microsoft.AspNetCore.Http;
using Sufinn.Visitor.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sufinn.Visitor.Repository
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public string GetTenant()
        {
            return _httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString() ?? "default";
        }
    }
}
