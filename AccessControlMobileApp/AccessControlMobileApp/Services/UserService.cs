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
using Xamarin.Essentials;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Google.Apis.Requests.BatchRequest;
using Google.Apis.Auth.OAuth2;

namespace AccessControlMobileApp.Services
{
    public class UserService
    {
        private FirebaseAuthClient userAuthClient;
        private FirebaseClient userDataClient;
        public Firebase.Auth.UserCredential UserAuthCredentials { get; private set; }

        private FirebaseAuthConfig config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyAHl-cJy8zniA0u7yQSu8hZ7avsToaAgEA",
            AuthDomain = "accesscontrolmobileapp.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        public UserService()
        {
            userDataClient = new FirebaseClient("https://accesscontrolmobileapp-default-rtdb.europe-west1.firebasedatabase.app/");
            userAuthClient = new FirebaseAuthClient(config);            
        }

        public async Task<string> LoginUser(string email, string password)
        {
            try
            {
                UserAuthCredentials = await userAuthClient.SignInWithEmailAndPasswordAsync(email, password);
                return null;
            }
            catch (FirebaseAuthHttpException ex)
            {
                return ParseErrorMessageFromResponse(ex);
            }
        }

        public async Task<string> RegisterUser(string email, string password, string username)
        {
            string result;
            try
            {
                UserAuthCredentials = await userAuthClient.CreateUserWithEmailAndPasswordAsync(email, password, username);
                result =  null;
            }
            catch (FirebaseAuthHttpException ex)
            {
                result =  ParseErrorMessageFromResponse(ex);
            }
            string userId = UserAuthCredentials.User.Uid;
            string path = $"users/{userId}";
            var user = new
            {
                Email = email,
                Username = username,
                IsAdmin = false,
                AccessLevel = 1,
                PreferedAccessMethod = 1
            };
            if (result == null )
            {
                try
                {
                    await userDataClient.Child(path).PutAsync(user);
                    result = null;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }

        public void LogoutUser()
        {
            userAuthClient.SignOut();
        }

        private string ParseErrorMessageFromResponse(FirebaseAuthHttpException ex)
        {
            JObject jsonResponse = JObject.Parse(ex.ResponseData);
            JArray errorsArray = (JArray)jsonResponse["error"]["errors"];

            string error = (string)errorsArray.First["message"];
            return error;
        }

    }
}
