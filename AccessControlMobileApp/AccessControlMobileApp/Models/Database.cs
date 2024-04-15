using Firebase.Database;
using Firebase.Auth;
using Firebase.Auth.Providers;

namespace AccessControlMobileApp.Models
{
    public class Database
    {
        protected FirebaseAuthClient userAuthClient;
        protected FirebaseClient databaseClient;

        private readonly FirebaseAuthConfig config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyAHl-cJy8zniA0u7yQSu8hZ7avsToaAgEA",
            AuthDomain = "accesscontrolmobileapp.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        public Database()
        {
            databaseClient = new FirebaseClient("https://accesscontrolmobileapp-default-rtdb.europe-west1.firebasedatabase.app/");
            userAuthClient = new FirebaseAuthClient(config);
        }
    }
}
