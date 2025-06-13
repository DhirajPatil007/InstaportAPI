
using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstaportApi.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly FcmService _fcmService;

        public NotificationController(FcmService fcmService)
        {
            _fcmService = fcmService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(string token)
        {

            Console.WriteLine("Device Token: " + token);
            var response = await _fcmService.SendNotificationAsync("Test Title", "Test Body", token);
            return Ok(new { messageId = response });
        }
    }
}



