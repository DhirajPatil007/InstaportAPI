using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
        {
            try
            {
                var existing = await _context.feedbacks
                    .FirstOrDefaultAsync(f => f.order_id == request.orderId && f.customer_id == request.customerId && f.feedback_type != "rider_to_customer");

                if (existing != null)
                {
                    return BadRequest(new { success = false, message = "Feedback already submitted for this order" });
                }

                var feedback = new feedbacks
                {
                    rider_id = request.riderId,
                    rider_name = request.riderName,
                    order_id = request.orderId,
                    customer_id = request.customerId,
                    rating = request.rating,
                    comments = request.comments,
                    feedback_type = "customer_to_rider",
                    createdAt = DateTime.UtcNow,
                };

                _context.feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { success = true, message = "Feedback submitted successfully", data = feedback });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error submitting feedback", error = ex.Message });
            }
        }

        [HttpGet("order/{orderId}/customer/{customerId}")]
        public async Task<IActionResult> GetFeedbackByOrder(Guid orderId, Guid customerId)
        {
            try
            {
                var feedback = await _context.feedbacks
                    .FirstOrDefaultAsync(f => f.order_id == orderId && f.customer_id == customerId && f.feedback_type != "rider_to_customer");

                if (feedback == null)
                {
                    return NotFound(new { success = false, message = "No feedback found for this order" });
                }

                return Ok(new { success = true, message = "Feedback fetched successfully", data = feedback });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching feedback", error = ex.Message });
            }
        }

        [HttpPost("rider-submit")]
        public async Task<IActionResult> SubmitRiderFeedback([FromBody] SubmitRiderFeedbackRequest request)
        {
            try
            {
                var existing = await _context.feedbacks
                    .FirstOrDefaultAsync(f => f.order_id == request.orderId && f.rider_id == request.riderId && f.feedback_type == "rider_to_customer");

                if (existing != null)
                {
                    return BadRequest(new { success = false, message = "Feedback already submitted for this order" });
                }

                var feedback = new feedbacks
                {
                    rider_id = request.riderId,
                    rider_name = request.riderName,
                    customer_id = request.customerId,
                    customer_name = request.customerName,
                    order_id = request.orderId,
                    rating = request.rating,
                    comments = request.comments,
                    feedback_type = "rider_to_customer",
                    createdAt = DateTime.UtcNow
                };

                _context.feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { success = true, message = "Feedback submitted successfully", data = feedback });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error submitting feedback", error = ex.Message });
            }
        }

        [HttpGet("order/{orderId}/rider/{riderId}")]
        public async Task<IActionResult> GetRiderFeedbackByOrder(Guid orderId, Guid riderId)
        {
            try
            {
                var feedback = await _context.feedbacks
                    .FirstOrDefaultAsync(f => f.order_id == orderId && f.rider_id == riderId && f.feedback_type == "rider_to_customer");

                if (feedback == null)
                {
                    return NotFound(new { success = false, message = "No feedback found for this order" });
                }

                return Ok(new { success = true, message = "Feedback fetched successfully", data = feedback });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching feedback", error = ex.Message });
            }
        }
    }

    // DTOs
    public class SubmitFeedbackRequest
    {
        public Guid riderId { get; set; }
        public string riderName { get; set; }
        public Guid orderId { get; set; }
        public Guid customerId { get; set; }
        public int rating { get; set; }
        public string comments { get; set; }
    }

    public class SubmitRiderFeedbackRequest
    {
        public Guid customerId { get; set; }
        public string customerName { get; set; }
        public Guid orderId { get; set; }
        public Guid riderId { get; set; }
        public string riderName { get; set; }
        public int rating { get; set; }
        public string comments { get; set; }
    }
}