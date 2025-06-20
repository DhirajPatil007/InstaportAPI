public class ValidateOtpResponseData
{
    public string verificationId { get; set; }
    public string mobileNumber { get; set; }
    public string responseCode { get; set; }
    public string errorMessage { get; set; }
    public string verificationStatus { get; set; }
    public string authToken { get; set; }
    public string transactionId { get; set; }
}

public class ValidateOtpResponse
{
    public int responseCode { get; set; }
    public string message { get; set; }
    public ValidateOtpResponseData data { get; set; }
}
