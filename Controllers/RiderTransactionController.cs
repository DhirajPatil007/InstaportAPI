using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/rider-transactions")]
    public class RiderTransactionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RiderTransactionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _context.rider_transactions
                .OrderByDescending(t => t.timestamp)
                .ToListAsync();

            return Ok(new { error = false, message = "All transactions fetched!", transactions });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _context.rider_transactions.FirstOrDefaultAsync(t => t._id == id);

            if (transaction == null)
            {
                return NotFound(new { error = true, message = "Transaction not found." });
            }

            return Ok(new { error = false, message = "Transaction fetched successfully.", transaction });
        }


        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? riderId)
        {
            IQueryable<rider_transactions> query = _context.rider_transactions;

            if (startDate.HasValue)
            {
                var startUnix = new DateTimeOffset(startDate.Value.Date).ToUnixTimeMilliseconds();
                query = query.Where(t => t.timestamp >= startUnix);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                var endUnix = new DateTimeOffset(endOfDay).ToUnixTimeMilliseconds();
                query = query.Where(t => t.timestamp <= endUnix);
            }

            if (riderId.HasValue)
            {
                query = query.Where(t => t.rider == riderId);
            }

            var filtered = await query.OrderByDescending(t => t.timestamp).ToListAsync();

            return Ok(new { error = false, message = "Filtered transactions fetched!", transactions = filtered });
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] rider_transactions updated)
        {
            if (updated == null || updated._id == Guid.Empty)
                return BadRequest(new { error = true, message = "Invalid transaction data." });

            var existing = await _context.rider_transactions
                .FirstOrDefaultAsync(t => t._id == updated._id);

            if (existing == null)
                return NotFound(new { error = true, message = "Transaction not found." });

            _context.Entry(existing).CurrentValues.SetValues(updated);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Transaction updated!", transaction = updated });
        }
    }
}
