public class OtpResponseData
{
    public string verificationId { get; set; }
    public string mobileNumber { get; set; }
    public string responseCode { get; set; }
    public string errorMessage { get; set; }
    public string timeout { get; set; }
    public string smCLI { get; set; }
    public string transactionId { get; set; }
}

public class OtpResponse
{
    public int responseCode { get; set; }
    public string message { get; set; }
    public OtpResponseData data { get; set; }
}
