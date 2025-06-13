using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
         private readonly AppDbContext _context;

        public CouponController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] coupons newCoupon)
        {
            try
            {
                var existingCoupon = await _context.coupons.FirstOrDefaultAsync(c => c.code == newCoupon.code);
                if (existingCoupon != null)
                {
                    return BadRequest(new { error = true, message = "Coupon code already exists!" });
                }

                _context.coupons.Add(newCoupon);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    error = false,
                    message = "Coupon created successfully!",
                    coupon = newCoupon
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var coupons = await _context.coupons.ToListAsync();

                return Ok(new
                {
                    error = false,
                    message = "Coupons fetched successfully!",
                    coupons
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpGet("{_id}")]
        public async Task<IActionResult> GetByCodeOrId(string _id)
        {
            try
            {
                var coupon = await _context.coupons
                    .FirstOrDefaultAsync(c => c.code == _id || c._id.ToString() == _id);

                if (coupon == null)
                {
                    return NotFound(new { error = true, message = "Coupon not found!" });
                }

                return Ok(new
                {
                    error = false,
                    message = "Coupon fetched successfully!",
                    coupon
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        [HttpPut("{_id}/update")]
        public async Task<IActionResult> Update(string _id, [FromBody] bool disabled)
        {
            try
            {
                var coupon = await _context.coupons.FirstOrDefaultAsync(c => c._id.ToString() == _id);
                if (coupon == null)
                {
                    return NotFound(new { error = true, message = "Coupon not found!" });
                }

                coupon.disabled = disabled;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    error = false,
                    message = "Coupon updated successfully!",
                    coupon
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }
    }
}