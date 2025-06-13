using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerAddressesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomerAddresses/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _context.customer_addresses
                .AsNoTracking()
                .ToListAsync();

            return Ok(new { error = false, message = "Customer addresses fetched successfully", addresses });
        }

        // GET: api/CustomerAddresses/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAddressesByCustomer(Guid customerId)
        {
            var addresses = await _context.customer_addresses
                .AsNoTracking()
                .Where(a => a.customer_id == customerId)
                .ToListAsync();

            if (addresses.Count == 0)
                return Ok(new { error = true, message = "No addresses found for this customer", addresses = new object[] { } });

            return Ok(new { error = false, message = "Addresses fetched successfully", addresses });
        }

        // PUT: api/CustomerAddresses/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAddress([FromBody] customer_addresses updatedAddress)
        {
            var existingAddress = await _context.customer_addresses
                .FirstOrDefaultAsync(a => a.customer_address_id == updatedAddress.customer_address_id);

            if (existingAddress == null)
                return NotFound(new { error = true, message = "Address not found" });

            updatedAddress.updatedAt = DateTime.UtcNow;
            _context.Entry(existingAddress).CurrentValues.SetValues(updatedAddress);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Address updated successfully", address = updatedAddress });
        }
    }
}
