using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Sufinn.Visitor.Controllers
{
    [Route("tenant")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TenantController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("validate-tenant")]
        public IActionResult ValidateTenant([FromQuery] string name)
        {
            var validTenants = _configuration.GetSection("ValidTenants").Get<List<string>>();

            if (string.IsNullOrEmpty(name))
                return BadRequest("Tenant is required.");

            if (validTenants.Contains(name, StringComparer.OrdinalIgnoreCase))
                return Ok();

            return NotFound("Invalid tenant.");
        }
    }
}
