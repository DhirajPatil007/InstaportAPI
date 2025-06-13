using FirebaseAdmin.Messaging;

public class FcmService
{
    public async Task<string> SendNotificationAsync(string title, string body, string deviceToken)
    {
        var message = new Message()
        {
            Token = deviceToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                { "customKey", "customValue" }
            }
        };

        return await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }
}
