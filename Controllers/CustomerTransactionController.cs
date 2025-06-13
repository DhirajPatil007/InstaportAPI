using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerTransactionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public CustomerTransactionController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("wallet-topup")]
        public async Task<IActionResult> WalletTopUp([FromBody] WalletTopUpRequest request)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(request.transaction_response);
                var customerId = Guid.Parse(token.Payload["additional_info1"].ToString());
                var amount = Convert.ToDecimal(token.Payload["amount"]);

                var customer = await _context.users.FindAsync(customerId);
                if (customer == null) return NotFound();

                customer.wallet += amount;
                await _context.SaveChangesAsync();

                var transaction = new customer_transactions
                {
                    customer_id = customerId,
                    payment_method_type = token.Payload["payment_method_type"].ToString(),
                    status = token.Payload["transaction_error_type"].ToString(),
                    amount = amount,
                    type = "topup",
                    wallet = true,
                    debit = false,
                    timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
                };
                await _context.customer_transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return Redirect("https://instaport-transactions.vercel.app/success.html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpPost("order-payment")]
        public async Task<IActionResult> CreateOrderTransaction([FromBody] OrderTransactionRequest request)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(request.transaction);

                var transaction = new customer_transactions
                {
                    customer_id = request.customerId,
                    payment_method_type = token.Payload["payment_method_type"].ToString(),
                    status = token.Payload["transaction_error_type"].ToString(),
                    amount = Convert.ToDecimal(token.Payload["amount"]),
                    type = "payment",
                    wallet = false,
                    debit = true,
                    timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
                };

                await _context.customer_transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return Ok(new { error = false, message = "Payment successful!", transaction });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpPost("wallet-order-payment")]
        public async Task<IActionResult> WalletOrderTransaction([FromBody] WalletOrderRequest request)
        {
            try
            {
                var customer = await _context.users.FindAsync(request.customerId);
                if (customer == null) return NotFound();

                var order = new orders
                {
                    customer = request.customerId,
                    amount = request.amount,
                    // Other order details...
                };
                _context.orders.Add(order);
                await _context.SaveChangesAsync();

                if (request.amount >= customer.holdAmount)
                {
                    var transaction = new customer_transactions
                    {
                        customer_id = request.customerId,
                        order_id = order._id,
                        payment_method_type = "wallet",
                        status = "success",
                        amount = request.amount,
                        type = "payment",
                        wallet = true,
                        debit = true,
                        timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
                    };
                    _context.customer_transactions.Add(transaction);

                    customer.wallet -= (request.amount - customer.holdAmount);
                    customer.holdAmount = 0;
                }
                else
                {
                    var transaction = new customer_transactions
                    {
                        customer_id = request.customerId,
                        order_id = order._id,
                        payment_method_type = "hold",
                        status = "success",
                        amount = request.amount,
                        type = "payment",
                        wallet = true,
                        debit = true,
                        timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
                    };
                    _context.customer_transactions.Add(transaction);

                    customer.holdAmount -= request.amount;
                }

                await _context.SaveChangesAsync();

                return Ok(new { error = false, message = "Payment successful!", order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.customer_transactions
                .OrderByDescending(t => t.timestamp)
                .ToListAsync();

            return Ok(new { error = false, message = "All transactions fetched successfully!", transactions });
        }


        [HttpGet("my-transactions/{customerId}")]
        public async Task<IActionResult> GetCustomerTransactions(Guid customerId)
        {
            var transactions = await _context.customer_transactions
                .Where(t => t.customer_id == customerId)
                .OrderByDescending(t => t.timestamp)
                .ToListAsync();

            return Ok(new { error = false, message = "Fetched successfully!", transactions });
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<IActionResult> GetTransactionById(Guid transactionId)
        {
            var transaction = await _context.customer_transactions
                .FirstOrDefaultAsync(t => t._id == transactionId);

            if (transaction == null)
            {
                return NotFound(new { error = true, message = "Transaction not found." });
            }

            return Ok(new { error = false, message = "Transaction fetched successfully!", transaction });
        }

        // [HttpGet("transaction/filtered")]
        // public async Task<IActionResult> GetFilteredTransactions(
        //     [FromQuery] DateTime? startDate,
        //     [FromQuery] DateTime? endDate,
        //     [FromQuery] string? search)
        // {
        //     IQueryable<customer_transactions> query = _context.customer_transactions;

        //     // Helper to convert DateTime to Unix epoch milliseconds
        //     long ToUnixMilliseconds(DateTime dt) =>
        //         (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

        //     if (startDate.HasValue)
        //     {
        //         var startUnix = ToUnixMilliseconds(startDate.Value);
        //         query = query.Where(t => t.timestamp.HasValue && t.timestamp.Value >= startUnix);
        //     }

        //     if (endDate.HasValue)
        //     {
        //         var endUnix = ToUnixMilliseconds(endDate.Value);
        //         query = query.Where(t => t.timestamp.HasValue && t.timestamp.Value <= endUnix);
        //     }

        //     if (!string.IsNullOrEmpty(search))
        //     {
        //         // Parse search as a date to filter by timestamp on that day
        //         if (DateTime.TryParse(search, out DateTime parsedDate))
        //         {
        //             var dayStartUnix = ToUnixMilliseconds(parsedDate.Date);
        //             var dayEndUnix = ToUnixMilliseconds(parsedDate.Date.AddDays(1).AddTicks(-1));

        //             query = query.Where(t =>
        //                 t.timestamp.HasValue &&
        //                 t.timestamp.Value >= dayStartUnix &&
        //                 t.timestamp.Value <= dayEndUnix);
        //         }
        //         else
        //         {
        //             // If search is not a valid date, return empty or do nothing (optional)
        //             // For example, you can return empty list:
        //             return Ok(new
        //             {
        //                 error = false,
        //                 message = "No transactions found for the given search input",
        //                 transactions = new List<customer_transactions>()
        //             });
        //         }
        //     }

        //     var filteredTransactions = await query.ToListAsync();

        //     return Ok(new
        //     {
        //         error = false,
        //         message = "Filtered transactions fetched successfully!",
        //         transactions = filteredTransactions
        //     });
        // }

        [HttpGet("transaction/filtered")]
        public async Task<IActionResult> GetFilteredTransactions(
          [FromQuery] DateTime? startDate,
          [FromQuery] DateTime? endDate,
          [FromQuery] string? search)
        {
            IQueryable<customer_transactions> query = _context.customer_transactions;

            // Convert DateTime to Unix epoch milliseconds
            long ToUnixMilliseconds(DateTime dt) =>
                (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

            if (startDate.HasValue)
            {
                var startUnix = ToUnixMilliseconds(startDate.Value.Date);
                query = query.Where(t => t.timestamp.HasValue && t.timestamp.Value >= startUnix);
            }

            if (endDate.HasValue)
            {
                var endUnix = ToUnixMilliseconds(endDate.Value.Date.AddDays(1).AddTicks(-1));
                query = query.Where(t => t.timestamp.HasValue && t.timestamp.Value <= endUnix);
            }

            if (!string.IsNullOrEmpty(search))
            {
                if (DateTime.TryParse(search, out DateTime parsedDate))
                {
                    var startUnix = ToUnixMilliseconds(parsedDate.Date);
                    var endUnix = ToUnixMilliseconds(parsedDate.Date.AddDays(1).AddTicks(-1));
                    query = query.Where(t => t.timestamp.HasValue && t.timestamp.Value >= startUnix && t.timestamp.Value <= endUnix);
                }
            }

            var filteredTransactions = await query.ToListAsync();

            return Ok(new
            {
                error = false,
                message = "Filtered transactions fetched successfully!",
                transactions = filteredTransactions
            });
        }

        [HttpPost("manual-log")]
        public async Task<IActionResult> ManualLogTransaction([FromBody] ManualLogRequest request)
        {
            if (string.IsNullOrEmpty(request.transactionId) || request.amount <= 0 || string.IsNullOrEmpty(request.type) || request.time_stamp == null)
            {
                return BadRequest(new { error = true, message = "Missing required fields" });
            }

            var transaction = new customer_transactions
            {
                customer_id = request.customerId,
                order_id = request.orderId,
                payment_method_type = request.type == "wallet" ? "wallet" : "Online",
                status = "success",
                amount = request.amount,
                type = "payment",
                wallet = request.type == "wallet",
                debit = true,
                timestamp = new DateTimeOffset(request.time_stamp).ToUnixTimeMilliseconds()
            };

            await _context.customer_transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { error = false, message = "Transaction logged successfully" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateTransaction([FromBody] customer_transactions updatedTransaction)
        {
            if (updatedTransaction == null || updatedTransaction._id == Guid.Empty)
                return BadRequest(new { error = true, message = "Invalid transaction data." });

            var existingTransaction = await _context.customer_transactions
                .FirstOrDefaultAsync(t => t._id == updatedTransaction._id);

            if (existingTransaction == null)
                return NotFound(new { error = true, message = "Transaction not found." });

            // Update the updatedAt timestamp if you have one (adjust property name accordingly)
            updatedTransaction.updatedAt = DateTime.UtcNow;

            // Use EF Core's SetValues to copy all values from updatedTransaction to existingTransaction
            _context.Entry(existingTransaction).CurrentValues.SetValues(updatedTransaction);

            await _context.SaveChangesAsync();

            // Return only the updatedTransaction or selectively send response fields if needed
            return Ok(new { error = false, message = "Transaction updated successfully", transaction = updatedTransaction });
        }


    }



    // DTOs
    public class WalletTopUpRequest
    {
        public string transaction_response { get; set; }
    }

    public class OrderTransactionRequest
    {
        public Guid customerId { get; set; }
        public string transaction { get; set; }
    }

    public class WalletOrderRequest
    {
        public Guid customerId { get; set; }
        public decimal amount { get; set; }
    }

    public class ManualLogRequest
    {
        public Guid customerId { get; set; }
        public Guid? orderId { get; set; }
        public string transactionId { get; set; }
        public decimal amount { get; set; }
        public string type { get; set; }
        public DateTime time_stamp { get; set; }
    }
}