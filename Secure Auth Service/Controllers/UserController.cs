using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Secure_Auth_Service.DTO;
using Secure_Auth_Service.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Secure_Auth_Service.Controllers
{
    [ApiController]
    [Route("api/UserController")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        ApplicationDbContext context;
       public UserController(ApplicationDbContext _context, IConfiguration config) {
            context = _context;
            _config = config;
        }

        [HttpPost("createUser")]
       public IActionResult Createuser(Register user)
        {
            if(user == null)
            {
                return BadRequest();
            }

            if (context.Users.Any(s => s.email.Equals(user.email)))
            {
                return Conflict("Email is already in use.");
            }
            if (user.password.Length<8)
            {
                return BadRequest("password should be more then 8 letters");
            }

            string hashedpassword = BCrypt.Net.BCrypt.HashPassword(user.password);
            var newuser = new users { 
            name = user.name,
            email = user.email,
            password_hash = hashedpassword,
            role = user.role,
            created_at = DateTime.Now
            };
          context.Users.Add(newuser);
           context.SaveChanges();
            return Ok("User registered successfully.");
        }


        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto) {

            if(loginDto == null)
            {
                return BadRequest("plz fill both email and password");
            }
            var user = context.Users.FirstOrDefault(s => s.email.Equals(loginDto.Email));
            if(user == null|| !BCrypt.Net.BCrypt.Verify(loginDto.Password,user.password_hash)) {
            return Unauthorized("Invalid credentials.");
            }
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
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
               HttpOnly = true,
               Secure = false, // its false because we are testin in local host, in production you will set it to true
               SameSite = SameSiteMode.Lax,
               Expires = DateTime.Now.AddMinutes(60)
            });

            return Ok("Login successful");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminPanel()
        {
            return Ok("Welcome, Admin!");
        }

        [Authorize(Roles ="user")]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok("Welcome to your dashboard!");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok("Logged out");
        }
    }


}
