using InstaportApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class MessageCentralOtpController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private const string BaseUrl = "https://cpaas.messagecentral.com";

    public MessageCentralOtpController(AppDbContext context)
    {
        _httpClient = new HttpClient();
           _context = context;
    }

    // 1. Generate Token
     [HttpGet("get-token")]
    public async Task<IActionResult> GetToken(
        [FromQuery] string customerId,
        [FromQuery] string password,
        [FromQuery] string email = "test@messagecentral.com",
        [FromQuery] string scope = "NEW",
        [FromQuery] int country = 91)
    {
        try
        {
            // 1. Encode password to Base64
            string base64Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            // 2. Build request URL
            string url = $"https://cpaas.messagecentral.com/auth/v1/authentication/token" +
                         $"?customerId={customerId}&key={base64Key}&scope={scope}&country={country}&email={email}";

            // 3. Send GET request
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "*/*");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                statusCode = response.StatusCode,
                rawResponse = responseContent
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = true, message = "Failed to generate token", exception = ex.Message });
        }
    }

    // 2. Send OTP
   [HttpPost("send-otp")]
public async Task<IActionResult> SendOtp(
    [FromQuery] string countryCode = "91",
    [FromQuery] string mobileNumber = "",
    [FromQuery] string flowType = "SMS",
    [FromQuery] int otpLength = 4)
{
    try
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            return BadRequest(new { error = true, message = "Mobile number is required." });
        }

        // 1. Get the latest token from the table
        var latestToken = await _context.MessageCentralToken
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => t.Token)
            .FirstOrDefaultAsync();

        Console.WriteLine("Token used:");
        Console.WriteLine(latestToken);

        if (string.IsNullOrEmpty(latestToken))
        {
            return BadRequest(new { error = true, message = "No auth token found in the database." });
        }

        // 2. Build URL
        var url = $"{BaseUrl}/verification/v3/send" +
                  $"?countryCode={Uri.EscapeDataString(countryCode)}" +
                  $"&flowType={Uri.EscapeDataString(flowType)}" +
                  $"&mobileNumber={Uri.EscapeDataString(mobileNumber)}" +
                  $"&otpLength={otpLength}";

        // 3. Make HTTP request
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("authToken", latestToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine("RAW RESPONSE:");
        Console.WriteLine(content);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                error = true,
                message = "OTP sending failed",
                details = content
            });
        }

        // 4. Deserialize and return success response
        var otpResponse = JsonConvert.DeserializeObject<OtpResponse>(content);
        return Ok(otpResponse);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            error = true,
            message = "Exception during OTP sending",
            exception = ex.Message
        });
    }
}


    // 3. Validate OTP
  [HttpGet("validate-otp")]
public async Task<IActionResult> ValidateOtp(
    [FromQuery] string verificationId,
    [FromQuery] string code,
    [FromQuery] string langId = "en")
{
    try
    {
        if (string.IsNullOrWhiteSpace(verificationId) || string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { error = true, message = "verificationId and code are required." });
        }

        // 1. Get latest token from the database
        var latestToken = await _context.MessageCentralToken
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => t.Token)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(latestToken))
        {
            return BadRequest(new { error = true, message = "No auth token found in the database." });
        }

        // 2. Build URL
        var url = $"{BaseUrl}/verification/v3/validateOtp" +
                  $"?verificationId={Uri.EscapeDataString(verificationId)}" +
                  $"&code={Uri.EscapeDataString(code)}" +
                  $"&langId={Uri.EscapeDataString(langId)}";

        // 3. Make HTTP request
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("authToken", latestToken);

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Validate OTP Raw Response:");
        Console.WriteLine(content);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                error = true,
                message = "OTP validation failed",
                details = content
            });
        }

        // 4. Deserialize and return success response
        var validationResult = JsonConvert.DeserializeObject<ValidateOtpResponse>(content);
        return Ok(validationResult);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            error = true,
            message = "Exception during OTP validation",
            exception = ex.Message
        });
    }
}


}
