using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
// using FirebaseAdmin.Messaging;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // Create Order
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] orders newOrder)
        {
            try
            {
                var orderId = await GenerateOrderId();
                newOrder.orderId = orderId;
                newOrder.amount = Math.Round(newOrder.amount ?? 0m);
                _context.orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return Ok(new { error = false, message = "Order Created Successfully", order = newOrder });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, message = ex.Message });
            }
        }

        // Generate Order ID
        private async Task<string> GenerateOrderId()
        {
            var now = DateTime.UtcNow;
            var day = now.Day.ToString("00");
            var month = now.Month.ToString("00");
            var datePart = $"{day}{month}";

            var todayStart = now.Date;
            var todayEnd = todayStart.AddDays(1).AddTicks(-1);

            var lastOrder = await _context.orders
                .Where(o => o.createdAt >= todayStart && o.createdAt <= todayEnd)
                .OrderByDescending(o => o.orderId)
                .FirstOrDefaultAsync();

            var sequence = 1;
            if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.orderId))
            {
                var lastSequence = int.Parse(lastOrder.orderId.Substring(6));
                sequence = lastSequence + 1;
            }

            var sequencePart = sequence.ToString("00000");
            return $"IP{datePart}{sequencePart}";
        }

        // Update Order
        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder([FromBody] orders updatedOrder)
        {
            var order = await _context.orders.FindAsync(updatedOrder._id);
            if (order == null)
                return NotFound(new { error = true, message = "Order not found" });

            _context.Entry(order).CurrentValues.SetValues(updatedOrder);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Order updated successfully!", order = updatedOrder });
        }

        // Order Status Update
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] orders statusUpdate)
        {
            var order = await _context.orders.FindAsync(id);
            if (order == null)
                return NotFound(new { error = true, message = "Order not found" });

            _context.Entry(order).CurrentValues.SetValues(statusUpdate);
            await _context.SaveChangesAsync();

            // // Example FCM notification
            // if (!string.IsNullOrEmpty(user.fcmtoken))
            // {
            //     var message = new Message
            //     {
            //         Notification = new Notification { Title = "Order Status", Body = $"Order {order._id} status updated" },
            //         Token = order.customerFcmtoken
            //     };
            //     await FirebaseMessaging.DefaultInstance.SendAsync(message);
            // }

            return Ok(new { error = false, message = "Order status updated", order });
        }

        // Get All Orders
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.orders.ToListAsync();
            return Ok(new { error = false, message = "Orders fetched", orders });
        }

        [HttpGet("left-join/all")]
        public async Task<IActionResult> GetAllOrdersByLeftJoin()
        {
            var orders = await (
                from o in _context.orders
                join u in _context.users on o.customer equals u._id into userGroup
                from user in userGroup.DefaultIfEmpty() // LEFT JOIN
                orderby o.createdAt descending
                select new
                {
                    o._id,
                    o.orderId,
                    o.delivery_type,
                    o.parcel_weight,
                    o.phone_number,
                    o.notify_sms,
                    o.courier_bag,
                    o.vehicle,
                    o.status,
                    o.payment_method,
                    o.package,
                    o.parcel_value,
                    o.amount,
                    o.commission,
                    o.reason,
                    o.time_stamp,
                    o.timer,
                    o.createdAt,
                    o.updatedAt,
                    o.rider,
                    customerId = o.customer,
                    customerName = user != null ? user.fullname : null
                }
            ).ToListAsync();

            return Ok(new { error = false, message = "Orders fetched with customer names", orders });
        }

        // Customer Orders
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> CustomerOrders(Guid customerId)
        {
            var orders = await _context.orders
                .Where(o => o.customer == customerId && o.status != "unpaid")
                .OrderByDescending(o => o.createdAt)
                .ToListAsync();
            return Ok(new { error = false, message = "Customer orders fetched", orders });
        }

        // Order by ID (for customer)
        [HttpGet("by-id/{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _context.orders
                .Where(o => o._id == orderId && o.status != "unpaid")
                .FirstOrDefaultAsync();
            if (order == null)
                return NotFound(new { error = true, message = "Order not found" });

            return Ok(new { error = false, message = "Order fetched", order });
        }

        [HttpGet("left-join/{orderId}")]
        public async Task<IActionResult> GetOrderByIdLeftJoin(Guid orderId)
        {
            var orderWithCustomer = await (
                from o in _context.orders
                join u in _context.users
                    on o.customer equals u._id into userGroup
                from customer in userGroup.DefaultIfEmpty() // left join
                where o._id == orderId
                select new
                {
                    order = o,
                    customerName = customer != null ? customer.fullname : null
                }
            ).FirstOrDefaultAsync();

            if (orderWithCustomer == null)
                return NotFound(new { error = true, message = "Order not found" });

            var result = orderWithCustomer.order;
            var response = new
            {
                _id = result._id,
                status = result.status,
                payment_method = result.payment_method,
                customer_id = result.customer,
                // add other order fields as needed
                customerName = orderWithCustomer.customerName,
                package = result.package,
                parcel_value = result.parcel_value,
                amount = result.amount,
                courier_bag = result.courier_bag,
                notify_sms = result.notify_sms,
                parcel_weight = result.parcel_weight,
                phone_number = result.phone_number,
                delivery_type = result.delivery_type,
                rider = result.rider,
            };

            return Ok(new { error = false, message = "Order fetched", order = response });
        }


        // Cancel Order
        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(Guid orderId, [FromBody] CancelOrderRequest request)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
                return NotFound(new { error = true, message = "Order not found" });

            order.status = "cancelled";
            order.reason = request.reason;

            await _context.SaveChangesAsync();

            // Example: deduct cancellation charges
            var customer = await _context.users.FindAsync(order.customer);
            if (customer != null)
            {
                customer.holdAmount += (order.amount - request.cancellationCharges);
                await _context.SaveChangesAsync();
            }

            return Ok(new { error = false, message = "Order cancelled", cancellationCharges = request.cancellationCharges });
        }

        // DTO for cancel order
        public class CancelOrderRequest
        {
            public string reason { get; set; }
            public decimal cancellationCharges { get; set; }
        }
    }
}