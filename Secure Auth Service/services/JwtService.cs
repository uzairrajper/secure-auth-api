using Microsoft.IdentityModel.Tokens;
using Secure_Auth_Service.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Secure_Auth_Service.services
{
    public class JwtService
    {
        private readonly IConfiguration? _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string CreateJwtTokens(users user)
        {
            var claims = new[]
            {

                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );
            return  new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
