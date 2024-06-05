using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IdentityJWT.Models.DTO;
using IdentityJWT.Utility.Interface;
using Microsoft.Extensions.Configuration;

namespace IdentityJWT.Utility
{
    public class JwtManager : IJwtManager
    {
        private readonly IConfiguration _config;

        public JwtManager(IConfiguration config)
        {
            _config = config;
        }
        public (string token, string refreshToken) GenerateJwtandRefreshToken(ApplicationUser user, List<string> roles)
        {
            string token = GenerateJwtToken(user, roles);
            string refreshToken = RefreshTokenGenerator();

            return (token, refreshToken);
        }


        public string GenerateJwtToken(ApplicationUser user, List<string>? roles = null)
        {
            var claims = new List<Claim>
            {
                new Claim("userID", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.Fname} {user.Lname}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            if(roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            //var jwt_secret = Environment.GetEnvironmentVariable("JWT_Secret") ?? throw new InvalidOperationException("no JWT secret set");
            var jwt_secret = _config["JWT_Secret"] ?? throw new InvalidOperationException("no JWT secret set");
            var key = Encoding.ASCII.GetBytes(jwt_secret); // Retrieve from configuration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6), // Set token expiration as needed
                //Expires = DateTime.UtcNow.AddSeconds(15), // Set token expiration as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string RefreshTokenGenerator()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var jwt_secret = Environment.GetEnvironmentVariable("JWT_Refresh_Secret") ?? throw new InvalidOperationException("no JWT secret set");
            var jwt_secret = _config["JWT_Refresh_Secret"] ?? throw new InvalidOperationException("no JWT secret set");
            var key = Encoding.ASCII.GetBytes(jwt_secret); // Retrieve from configuration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = null,
                Expires = DateTime.UtcNow.AddDays(15), // Set token expiration as needed                
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> RefreshTokenValidate(string refreshToken)
        {
            //var jwt_secret = Environment.GetEnvironmentVariable("JWT_Refresh_Secret") ?? throw new InvalidOperationException("no JWT secret set");
            var jwt_secret = _config["JWT_Refresh_Secret"] ?? throw new InvalidOperationException("no JWT secret set");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_secret)), // Ensure your key is secure
                ValidateIssuer = false,
                ValidateAudience = false
            };
            try
            {
                var result = await tokenHandler.ValidateTokenAsync(refreshToken, validationParameters);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}
