
using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
// using FirebaseAdmin.Messaging;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<riders> _passwordHasher;
        private readonly IConfiguration _config;

        public RiderController(AppDbContext context, IPasswordHasher<riders> passwordHasher, IConfiguration config)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] riders newRider)
        {
            newRider.password = _passwordHasher.HashPassword(newRider, newRider.password);
            _context.riders.Add(newRider);
            await _context.SaveChangesAsync();
            return Ok(new { error = false, message = "Signup successful!", rider = newRider });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] riders loginRider)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r.mobileno == loginRider.mobileno);
            if (rider == null)
                return Ok(new { error = true, message = "Rider not found" });

            var result = _passwordHasher.VerifyHashedPassword(rider, rider.password, loginRider.password);
            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(rider);
                rider.token = token;
                rider.fcmtoken = loginRider.fcmtoken;
                await _context.SaveChangesAsync();
                return Ok(new { error = false, message = "Logged In Successfully", token });
            }
            else
                return Ok(new { error = true, message = "Invalid Credentials" });
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] riders updatedRider)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r._id == updatedRider._id);
            if (rider == null) return NotFound(new { error = true, message = "Rider not found" });
            _context.Entry(rider).CurrentValues.SetValues(updatedRider);
            await _context.SaveChangesAsync();
            return Ok(new { error = false, message = "Profile updated successfully!", rider = updatedRider });
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateRequest request)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r.mobileno == request.mobileno);
            if (rider == null) return NotFound(new { error = true, message = "Rider not found" });
            rider.password = _passwordHasher.HashPassword(rider, request.password);
            await _context.SaveChangesAsync();
            return Ok(new { error = false, message = "Password updated successfully!" });
        }

        [HttpGet("me/{riderId}")]
        public async Task<IActionResult> GetProfile(Guid riderId)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r._id == riderId);
            if (rider == null) return NotFound(new { error = true, message = "Rider not found" });
            return Ok(new { error = false, message = "Profile fetched", rider });
        }

        [HttpGet("all")]
        public async Task<IActionResult> AllRiders()
        {
            var riders = await _context.riders.AsNoTracking().ToListAsync();
            riders.ForEach(r => r.password = null);
            return Ok(new { error = false, message = "Riders fetched successfully", riders });
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> AllRiders(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? search)
        {
            IQueryable<riders> query = _context.riders.AsNoTracking();

            // Convert DateTime to Unix epoch milliseconds
            long ToUnixMilliseconds(DateTime dt) =>
                (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            if (startDate.HasValue)
            {
                var startUnix = ToUnixMilliseconds(startDate.Value.Date);
                query = query.Where(r => r.timestamp.HasValue && r.timestamp.Value >= startUnix);
            }

            if (endDate.HasValue)
            {
                var endUnix = ToUnixMilliseconds(endDate.Value.Date.AddDays(1).AddTicks(-1));
                query = query.Where(r => r.timestamp.HasValue && r.timestamp.Value <= endUnix);
            }

            // Search parameter: filter by date only
            if (!string.IsNullOrEmpty(search))
            {
                if (DateTime.TryParse(search, out DateTime parsedDate))
                {
                    var startUnix = ToUnixMilliseconds(parsedDate.Date);
                    var endUnix = ToUnixMilliseconds(parsedDate.Date.AddDays(1).AddTicks(-1));
                    query = query.Where(r => r.timestamp.HasValue && r.timestamp.Value >= startUnix && r.timestamp.Value <= endUnix);
                }
                // else do nothing if search is invalid date
            }

            var riders = await query.ToListAsync();

            // Hide passwords before returning
            riders.ForEach(r => r.password = null);

            return Ok(new
            {
                error = false,
                message = "Riders fetched successfully",
                riders
            });
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusUpdateRequest request)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r._id == request.riderId);
            if (rider == null) return NotFound(new { error = true, message = "Rider not found" });
            rider.status = request.status;
            await _context.SaveChangesAsync();
            return Ok(new { error = false, message = "Status updated successfully!" });
        }

        [HttpPost("order-assign/{orderId}")]
        public async Task<IActionResult> AssignOrder(Guid orderId, [FromBody] OrderAssignRequest request)
        {
            var order = await _context.orders.FirstOrDefaultAsync(o => o._id == orderId);
            if (order == null) return NotFound(new { error = true, message = "Order not found" });
            order.rider = request.riderId;
            order.status = "processing";
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(request.fcmtoken))
            {
                // Uncomment the following lines to send a notification using Firebase Cloud Messaging
                // var message = new Message
                // {
                //     Notification = new Notification { Title = "Order Assigned", Body = "New order assigned to you" },
                //     Token = request.fcmtoken
                // };
                // await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            return Ok(new { error = false, message = "Order assigned successfully!", order });
        }

        [HttpGet("transactions/{riderId}")]
        public async Task<IActionResult> GetTransactions(Guid riderId)
        {
            var transactions = await _context.rider_transactions.Where(t => t.rider == riderId)
                .OrderByDescending(t => t.timestamp).ToListAsync();
            return Ok(new { error = false, message = "Transactions fetched", transactions });
        }

        [HttpPost("request-amount")]
        public async Task<IActionResult> RequestAmount([FromBody] RequestAmountRequest request)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r._id == request.riderId);
            if (rider == null) return NotFound(new { error = true, message = "Rider not found" });
            rider.requestedAmount = request.amount;
            await _context.SaveChangesAsync();
            return Ok(new { error = false, message = "Requested amount updated successfully!" });
        }

        [HttpPost("pay-dues")]
        public async Task<IActionResult> PayDues([FromBody] PayDuesRequest request)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r._id == request.riderId);
            if (rider == null) return NotFound(new { error = true, message = "Rider not found" });
            rider.wallet_amount -= request.amount;
            rider.isDue = false;
            await _context.SaveChangesAsync();
            return Ok(new { error = false, message = "Dues paid successfully!" });
        }

        [HttpPost("check-validity")]
        public async Task<IActionResult> CheckValidity([FromBody] ValidityCheckRequest request)
        {
            var rider = await _context.riders.FirstOrDefaultAsync(r => r.mobileno == request.mobileno);
            if (rider == null) return NotFound(new { error = true, message = "No such rider exists" });
            return Ok(new { error = false, message = "Rider found!" });
        }

        private string GenerateJwtToken(riders rider)
        {
            var claims = new[] { new Claim("_id", rider._id.ToString()), new Claim("role", "Rider") };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class PasswordUpdateRequest { public string mobileno { get; set; } public string password { get; set; } }
    public class StatusUpdateRequest { public Guid riderId { get; set; } public string status { get; set; } }
    public class OrderAssignRequest { public Guid riderId { get; set; } public string fcmtoken { get; set; } }
    public class RequestAmountRequest { public Guid riderId { get; set; } public decimal amount { get; set; } }
    public class PayDuesRequest { public Guid riderId { get; set; } public decimal amount { get; set; } }
    public class ValidityCheckRequest { public string mobileno { get; set; } }
}
