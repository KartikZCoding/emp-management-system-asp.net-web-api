using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly SigningCredentials _signingCredentials;
        private readonly RsaSecurityKey _publicKey;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            // Load PRIVATE key once at startup. used for signing tokens
            var privateRsa = RSA.Create();
            privateRsa.ImportFromPem(File.ReadAllText(configuration["Jwt:PrivateKeyPath"]));
            _signingCredentials = new SigningCredentials(
                new RsaSecurityKey(privateRsa), SecurityAlgorithms.RsaSha256);

            // Load PUBLIC key once at startup. used for validating expired tokens
            var publicRsa = RSA.Create();
            publicRsa.ImportFromPem(File.ReadAllText(configuration["Jwt:PublicKeyPath"]));
            _publicKey = new RsaSecurityKey(publicRsa);
        }

        public string GenerateToken(int userId, string username, string email, List<string> permissions)
        {
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
               new Claim(ClaimTypes.Name, username),
               new Claim(ClaimTypes.Email, email),
            };

            // Add each permission as a separate claim
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: _signingCredentials
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
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,   // KEY: we allow expired tokens here
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = _publicKey,
                ClockSkew = TimeSpan.Zero
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
