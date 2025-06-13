using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
         private readonly AppDbContext _context;
        private readonly IPasswordHasher<admin> _passwordHasher;
        private readonly IConfiguration _config;

        public AdminController(AppDbContext context, IPasswordHasher<admin> passwordHasher, IConfiguration config)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] admin newAdmin)
        {
            try
            {
                newAdmin.password = _passwordHasher.HashPassword(newAdmin, newAdmin.password);

                _context.admin.Add(newAdmin);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    error = false,
                    message = "Account Created Successfully",
                    response = newAdmin
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = true,
                    message = ex.Message,
                    response = (object)null
                });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] admin loginAdmin)
        {
            var admin = await _context.admin.FirstOrDefaultAsync(a => a.username == loginAdmin.username);
            if (admin == null)
            {
                return Ok(new { error = true, message = "Something Went Wrong", admin = (object)null });
            }

            var verify = _passwordHasher.VerifyHashedPassword(admin, admin.password, loginAdmin.password);
            if (verify == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(admin);
                return Ok(new
                {
                    error = false,
                    message = "Logged In Successfully",
                    token
                });
            }
            else
            {
                return Ok(new { error = true, message = "Invalid Credentials", token = (object)null });
            }
        }

        private string GenerateJwtToken(admin admin)
        {
            var claims = new[]
            {
                new Claim("_id", admin._id.ToString()),
                new Claim("role", admin.role ?? "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}