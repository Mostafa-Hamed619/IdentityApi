using AdminFullStack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdminFullStack.Services
{
    public class JWTServices
    {
        private readonly IConfiguration config;
        private readonly UserManager<User> userManager;
        private readonly SymmetricSecurityKey JwtKey;
        public JWTServices(IConfiguration config,UserManager<User> userManager)
        {
            this.config = config;
            this.userManager = userManager;

            // jwtKey is used for encripting and decripting the jwt token
            this.JwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
        }
        public async Task<string> CreateJWT(User user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.UserName),
                new Claim(ClaimTypes.GivenName,user.FirstName),
                new Claim(ClaimTypes.Surname,user.LastName),
                new Claim("My own claim name","this is the value")
            };

            var roles = await userManager.GetRolesAsync(user);
            userClaims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));
            var credentials = new SigningCredentials(JwtKey, SecurityAlgorithms.HmacSha256);

            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(config["JWT:ExpiresInDays"])),
                SigningCredentials = credentials,
                Issuer = config["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDiscriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
