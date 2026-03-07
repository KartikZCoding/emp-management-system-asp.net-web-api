using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace Infrastructure.Services
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string username, string email, string rolename)
        {
            var privateKey = File.ReadAllText(_configuration["Jwt:PrivateKeyPath"]);

            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            var key = new RsaSecurityKey(rsa);

            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
               new Claim(ClaimTypes.Name, username),
               new Claim(ClaimTypes.Email, email),
               new Claim(ClaimTypes.Role, rolename),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var publicKeyText = File.ReadAllText(_configuration["Jwt:PublicKeyPath"]);
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyText);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,   // KEY: we allow expired tokens here
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
