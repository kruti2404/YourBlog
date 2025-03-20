using BlogProject.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogProject.Services
{
    public class JwtServices
    {

        private readonly IConfiguration _config;
        

        public JwtServices(IConfiguration config, JwtSecurityTokenHandler tokenHandler)
        {
            _config = config;
            
        }

        public string GenerateToken(User user)
        {
            var _tokenHandler = new JwtSecurityTokenHandler();
            var Claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: Claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials
                );

            return _tokenHandler.WriteToken(token);


        }




    }
}
