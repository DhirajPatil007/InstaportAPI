using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderAddressesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderAddressesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orderaddresses/get-all
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _context.order_addresses
                .Include(a => a.order)
                .ToListAsync();

            return Ok(new { error = false, message = "All addresses fetched successfully!", addresses });
        }

        // GET: api/orderaddresses/by-order/{orderId}
        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetAddressesByOrderId(Guid orderId)
        {
            var addresses = await _context.order_addresses
                .Include(a => a.order)
                .Where(a => a.order_id == orderId)
                .ToListAsync();

            if (addresses == null || addresses.Count == 0)
                return NotFound(new { error = true, message = "No addresses found for this order." });

            return Ok(new { error = false, message = "Addresses fetched successfully!", addresses });
        }

        // PUT: api/orderaddresses/update
        // Update expects full order_address object with order_address_id set
        // [HttpPut("update")]
        // public async Task<IActionResult> UpdateAddress([FromBody] order_addresses updatedAddress)
        // [HttpPut("update")]
        // public async Task<IActionResult> UpdateAddress([FromBody] List<order_addresses> updatedAddress)

        // {
        //     if (updatedAddress == null || updatedAddress.order_address_id == Guid.Empty)
        //         return BadRequest(new { error = true, message = "Invalid address data." });

        //     var existingAddress = await _context.order_addresses
        //         .FirstOrDefaultAsync(a => a.order_address_id == updatedAddress.order_address_id);

        //     if (existingAddress == null)
        //         return NotFound(new { error = true, message = "Address not found." });

        //     _context.Entry(existingAddress).CurrentValues.SetValues(updatedAddress);

        //     await _context.SaveChangesAsync();

        //     return Ok(new { error = false, message = "Address updated successfully", address = updatedAddress });
        // }
        // [HttpPut("update")]
        // public async Task<IActionResult> UpdateAddress([FromBody] List<order_addresses> updatedAddresses)
        // {
        //     if (updatedAddresses == null || updatedAddresses.Count == 0)
        //         return BadRequest(new { error = true, message = "Invalid address data." });

        //     foreach (var updatedAddress in updatedAddresses)
        //     {
        //         if (string.IsNullOrEmpty(updatedAddress.address))
        //             return BadRequest(new { error = true, message = "Address is required." });

        //         var existingAddress = await _context.order_addresses
        //             .FirstOrDefaultAsync(a => a.address == updatedAddress.address);

        //         if (existingAddress == null)
        //             return NotFound(new { error = true, message = $"Address not found: {updatedAddress.address}" });

        //         _context.Entry(existingAddress).CurrentValues.SetValues(updatedAddress);
        //     }

        //     await _context.SaveChangesAsync();
        //     return Ok(new { error = false, message = "Addresses updated successfully." });
        // }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAddress([FromBody] List<order_addresses> updatedAddresses)
        {
            if (updatedAddresses == null || updatedAddresses.Count == 0)
                return BadRequest(new { error = true, message = "Invalid address data." });

            foreach (var updatedAddress in updatedAddresses)
            {
                if (string.IsNullOrEmpty(updatedAddress.address))
                    return BadRequest(new { error = true, message = "Address is required." });

                var existingAddress = await _context.order_addresses
                    .FirstOrDefaultAsync(a => a.order_address_id == updatedAddress.order_address_id);

                if (existingAddress == null)
                    return NotFound(new { error = true, message = $"Address not found: {updatedAddress.order_address_id}" });

                _context.Entry(existingAddress).CurrentValues.SetValues(updatedAddress);
            }

            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Addresses updated successfully." });
        }


    }
}
