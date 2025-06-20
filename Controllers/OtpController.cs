using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/otp")]
    public class OtpController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        // TODO: Replace with your actual MSG91 Auth Key and Template ID
        private readonly string _authKey = "YOUR_MSG91_AUTH_KEY";
        private readonly string _templateId = "YOUR_TEMPLATE_ID";

        public OtpController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://api.msg91.com/api/v5/otp";

            var payload = new
            {
                template_id = _templateId,
                mobile = request.Mobile,
                otp_length = "6",
                otp_expiry = "5"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("authkey", _authKey);

            var response = await client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var client = _httpClientFactory.CreateClient();

            var url = $"https://api.msg91.com/api/v5/otp/verify?otp={request.Otp}&mobile={request.Mobile}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("authkey", _authKey);

            var response = await client.SendAsync(httpRequest);
            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
    }

    public class SendOtpRequest
    {
        public string Mobile { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Mobile { get; set; }
        public string Otp { get; set; }
    }
}
