using AccessControlMobileApp.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;

using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;

namespace AccessControlMobileApp.Services
{
    public class UserService
    {
        FirebaseClient client;

        FirebaseAuthConfig config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyAHl-cJy8zniA0u7yQSu8hZ7avsToaAgEA",
            AuthDomain = "dasda",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        FirebaseAuthClient clientclient;

        public UserService()
        {
            client = new FirebaseClient("https://accesscontrolmobileapp-default-rtdb.europe-west1.firebasedatabase.app/");
            clientclient = new FirebaseAuthClient(config);            
        }

        public async Task<bool> UserExists(string username)
        {
            var user = (await client.Child("Users")
                .OnceAsync<Models.User>()).Where(u => u.Object.Username == username).FirstOrDefault();
            return user != null;
        }

        public async Task<bool> RegisterUser(string username, string password)
        {
            if (await UserExists(username) == false)
            {
                await client.Child("Users").PostAsync(new Models.User()
                {
                    Username = username,
                    Password = password
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> LoginUser(string username, string password)
        {
            var user = (await client.Child("Users")
                .OnceAsync<Models.User>()).Where(u => u.Object.Username == username)
                .Where(u => u.Object.Password == password).FirstOrDefault();
            return user != null;
        }
    }
}
