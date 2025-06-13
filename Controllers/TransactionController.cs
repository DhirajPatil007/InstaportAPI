using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] TopUpRequest request)
        {
            try
            {
                var user = await _context.users.FindAsync(request.userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                var transaction = new transactions
                {
                    _id = Guid.NewGuid(),
                    userId = request.userId,
                    amount = request.amount,
                    razorpayPaymentId = request.razorpay_payment_id,
                    type = "topup",
                    // debit = false,
                    createdAt = DateTime.UtcNow
                };

                _context.transactions.Add(transaction);

                user.wallet += request.amount;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, transaction });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("my-transactions/{userId}")]
        public async Task<IActionResult> GetTransactions(Guid userId)
        {
            try
            {
                var transactions = await _context.transactions
                    .Where(t => t.userId == userId && (t.type == "topup" || t.type == "online"))
                    .OrderByDescending(t => t.createdAt)
                    .ToListAsync();

                return Ok(new { transactions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = "Failed to fetch transactions", details = ex.Message });
            }
        }
    }

    // DTO for TopUp request
    public class TopUpRequest
    {
        public Guid userId { get; set; }
        public decimal amount { get; set; }
        public string razorpay_payment_id { get; set; }
    }

}