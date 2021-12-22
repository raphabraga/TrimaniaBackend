using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes("trimania-jwt-authentication-key");
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("AuthKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Login),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, (user.Login == "admin") ? "Administrator" : "client")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}