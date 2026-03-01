using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

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
    }
}
