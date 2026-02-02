using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sufinn.Visitor.Repository.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Sufinn.Visitor.Repository
{
    public class JWTManagerRepository : IJWTManagerRepository
    {
        private readonly IConfiguration _configuration;
        public JWTManagerRepository(IConfiguration iconfiguration)
        {
            _configuration = iconfiguration;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
        public JwtSecurityToken CreateToken(List<Claim> claims)
        {
            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);
            int tokenValidityInMinutes = int.Parse(_configuration["Jwt:TokenValidityInMinutes"]);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
