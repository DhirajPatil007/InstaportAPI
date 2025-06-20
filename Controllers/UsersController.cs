using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<users> _passwordHasher;
        private readonly IConfiguration _config;

        public UsersController(AppDbContext context, IPasswordHasher<users> passwordHasher, IConfiguration config)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] users newUser)
        {
            try
            {
                var rider = await _context.riders.FirstOrDefaultAsync(r => r.mobileno == newUser.mobileno);
                if (rider != null)
                {
                    return StatusCode(403, new { error = true, message = "Cannot signup with this mobile number" });
                }

                newUser.password = _passwordHasher.HashPassword(newUser, newUser.password);

                _context.users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { error = false, message = "Account Created Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] users loginUser)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.mobileno == loginUser.mobileno);
            if (user == null)
            {
                return Ok(new { error = true, message = "Account not found", token = "" });
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.password, loginUser.password);
            if (verify == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(user);
                user.token = token;
                user.fcmtoken = loginUser.fcmtoken;

                await _context.SaveChangesAsync();

                return Ok(new { error = false, message = "Logged In Successfully", token });
            }
            else
            {
                return Ok(new { error = true, message = "Wrong id or password", token = "" });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] users updatedUser)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u._id == updatedUser._id);
            if (user == null)
                return Ok(new { error = true, message = "Something Went Wrong", user = (object)null });

            try
            {
                _context.Entry(user).CurrentValues.SetValues(updatedUser);
                await _context.SaveChangesAsync();

                return Ok(new { error = false, message = "Updated Successful!", user = updatedUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpPut("update-holdamount/{id}")]
        public async Task<IActionResult> UpdateHoldAmount(Guid id, [FromBody] decimal holdAmount)
        {
            var user = await _context.users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });
 
            user.holdAmount = holdAmount;
            user.updatedAt = DateTime.UtcNow;
 
            await _context.SaveChangesAsync();
 
            return Ok(new { message = "Hold amount updated successfully." });
        }

        [HttpGet("me/{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _context.users
                .Where(u => u._id == userId)
                // .Select(u => new { u._id, u.mobileno, u.role, u.fcmtoken, u.token })
                .FirstOrDefaultAsync();

            if (user == null)
                return Ok(new { error = true, message = "Something Went Wrong", user = (object)null });

            return Ok(new { error = false, message = "Fetched Successfully!", user });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.users
                // .Select(u => new { u._id, u.mobileno, u.role, u.fcmtoken })
                .ToListAsync();

            return Ok(new { error = false, message = "Users Fetched Successfully!", user = users });
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredUsers(
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] string? search)
        {
            IQueryable<users> query = _context.users.AsNoTracking();

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(u => u.createdAt >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1); // end of day
                query = query.Where(u => u.createdAt <= end);
            }

            if (!string.IsNullOrEmpty(search))
            {
                if (DateTime.TryParse(search, out DateTime parsedDate))
                {
                    var start = parsedDate.Date;
                    var end = parsedDate.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(u => u.createdAt >= start && u.createdAt <= end);
                }
            }

            var users = await query.ToListAsync();

            return Ok(new
            {
                error = false,
                message = "Filtered users fetched successfully!",
                user = users
            });
        }

        [HttpPost("check-validity")]
        public async Task<IActionResult> CheckValidity([FromBody] users userToCheck)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.mobileno == userToCheck.mobileno);
            if (user == null)
                return NotFound(new { error = true, message = "No such user exist" });

            return Ok(new { error = false, message = "User found" });
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] users updatedUser)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.mobileno == updatedUser.mobileno);
            if (user == null)
                return Ok(new { error = true, message = "Something Went Wrong", user = (object)null });

            user.password = _passwordHasher.HashPassword(user, updatedUser.password);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Password reset successfully!", user });
        }

        private string GenerateJwtToken(users user)
        {
            var claims = new[]
            {
                new Claim("_id", user._id.ToString()),
                new Claim("role", user.role ?? "User")
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