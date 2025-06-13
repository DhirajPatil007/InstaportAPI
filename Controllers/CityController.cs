using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] cities newCity)
        {
            try
            {
                _context.cities.Add(newCity);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    error = false,
                    message = "City Added Successfully",
                    city = newCity
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = true,
                    message = ex.Message,
                    city = (object)null
                });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var cities = await _context.cities.ToListAsync();

                if (cities.Any())
                {
                    return Ok(new
                    {
                        error = false,
                        message = "City Fetch Successfully",
                        city = cities
                    });
                }
                else
                {
                    return Ok(new
                    {
                        error = true,
                        message = "Something Went Wrong",
                        city = (object)null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = true,
                    message = ex.Message,
                    city = (object)null
                });
            }
        }
    }
}