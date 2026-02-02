using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sufinn.Visitor.Repository.Interface
{
    public interface IJWTManagerRepository
    {
        JwtSecurityToken CreateToken(List<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
