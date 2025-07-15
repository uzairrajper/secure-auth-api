using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Secure_Auth_Service.DTO;
using Secure_Auth_Service.Model;
using Secure_Auth_Service.services;
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
        JwtService _jwtService; 
        public UserController(ApplicationDbContext _context, IConfiguration config) {
            context = _context;
            _config = config;
            _jwtService = new JwtService(config);
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

            var tokenString = _jwtService.CreateJwtTokens(user);

            var refreshToken = Guid.NewGuid().ToString(); // or use RandomNumberGenerator
            var refreshTokenExpire = DateTime.UtcNow.AddDays(7);

            var refreshTokenEntry = new refresh_tokens
            {
                token = refreshToken,
                user_id = user.id,
                expires_at = refreshTokenExpire,
                is_revoked = 0,
                created_at = DateTime.UtcNow
            };
            context.Refresh_Tokens.Add(refreshTokenEntry);
            context.SaveChanges();
            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
               HttpOnly = true,
               Secure = false, // its false because we are testin in local host, in production you will set it to true
               SameSite = SameSiteMode.Lax,
               Expires = DateTime.Now.AddMinutes(60)
            });
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // its false because we are testin in local host, in production you will set it to true
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddMinutes(60)
            });

            return Ok("Login successful");
        }


        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            //first we have to check if 
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No refresh token provided.");

            var storedToken = context.Refresh_Tokens.FirstOrDefault(s=> s.token == refreshToken);
            if (storedToken == null || storedToken.expires_at < DateTime.UtcNow || storedToken.is_revoked == 1)
                return Unauthorized("Invalid or expired refresh token.");

            var user = context.Users.FirstOrDefault(s => s.id == storedToken.user_id);
            if (user == null)
                return Unauthorized("User not found.");

            //Rotate refresh token
            storedToken.is_revoked = 1;
            var newRefreshToken = Guid.NewGuid().ToString();
            var newExpires = DateTime.UtcNow.AddDays(7);
            context.Refresh_Tokens.Add(new refresh_tokens
            {
                token = newRefreshToken,
                user_id = user.id,
                created_at = DateTime.UtcNow,
                expires_at = newExpires,
                is_revoked = 0
            });
            context.SaveChanges();

            // created new token 
            var accessTokenString = _jwtService.CreateJwtTokens(user);

            Response.Cookies.Append("access_token", accessTokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // its false because we are testin in local host, in production you will set it to true
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddMinutes(60)
            });
            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // its false because we are testin in local host, in production you will set it to true
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddMinutes(60)
            });


            return Ok();
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
            Response.Cookies.Delete("refresh_token");
            return Ok("Logged out");
        }
    }


}
