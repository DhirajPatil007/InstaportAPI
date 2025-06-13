using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceManipulationController : ControllerBase
    {
         private readonly AppDbContext _context;

        public PriceManipulationController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/PriceManipulation
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] price_manipulations priceManipulation)
        {
            try
            {
                _context.price_manipulations.Add(priceManipulation);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    error = false,
                    message = "Price Added Successfully",
                    priceManipulation
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = true,
                    message = ex.Message
                });
            }
        }

        // PUT: api/PriceManipulation
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] price_manipulations updatedPriceManipulation)
        {
            var price = await _context.price_manipulations
                .FirstOrDefaultAsync(p => p._id == updatedPriceManipulation._id);

            if (price == null)
            {
                return NotFound(new
                {
                    error = true,
                    message = "Something Went Wrong",
                    priceManipulation = (object)null
                });
            }

            try
            {
                // Update only the fields provided in the request body
                _context.Entry(price).CurrentValues.SetValues(updatedPriceManipulation);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    error = false,
                    message = "Updated Successful!",
                    priceManipulation = updatedPriceManipulation
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = true,
                    message = ex.Message
                });
            }
        }

        // GET: api/PriceManipulation
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var priceManipulation = await _context.price_manipulations.FirstOrDefaultAsync();
                if (priceManipulation != null)
                {
                    return Ok(new
                    {
                        error = false,
                        message = "Price Fetch Successfully",
                        priceManipulation
                    });
                }
                else
                {
                    return Ok(new
                    {
                        error = true,
                        message = "Something Went Wrong",
                        priceManipulation = (object)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = true,
                    message = ex.Message
                });
            }
        }
    }
}