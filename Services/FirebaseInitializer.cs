using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

public static class FirebaseInitializer
{
    public static void InitializeFirebase()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("Secrets/instaport-test-5fa94-firebase-adminsdk-fbsvc-fe38cad64a.json") // use full path if needed
            });
        }
    }
}
