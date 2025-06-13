using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryStatusController : ControllerBase
    {
        //     private readonly AppDbContext _context;

        //     public DeliveryStatusController(AppDbContext context)
        //     {
        //         _context = context;
        //     }

        //     [HttpPost("create")]
        //     public async Task<IActionResult> Create([FromBody] delivery_status newStatus)
        //     {
        //         try
        //         {
        //             _context.delivery_status.Add(newStatus);
        //             await _context.SaveChangesAsync();

        //             return Ok(new
        //             {
        //                 error = false,
        //                 message = "Delivery Status Added",
        //                 status = newStatus
        //             });
        //         }
        //         catch (Exception ex)
        //         {
        //             return StatusCode(500, new { error = true, message = ex.Message });
        //         }
        //     }

        //     [HttpPost("by-order")]
        //     public async Task<IActionResult> GetByOrder([FromBody] OrderIdRequest request)
        //     {
        //         try
        //         {
        //             var statusList = await _context.delivery_status
        //                 .Where(d => d.order_id == request._id)
        //                 .OrderByDescending(d => d.timestamp)
        //                 .ToListAsync();

        //             return Ok(new
        //             {
        //                 error = false,
        //                 message = "Delivery Status Fetched",
        //                 status = statusList
        //             });
        //         }
        //         catch (Exception ex)
        //         {
        //             return StatusCode(500, new { error = true, message = ex.Message });
        //         }
        //     }
        // }

        // // Helper DTO for receiving _id in POST body

        // public class OrderIdRequest
        // {
        //     public Guid _id { get; set; }
        // }
    }
}