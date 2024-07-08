using AccessControlMobileApp.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database.Query;
using Firebase.Auth;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
using FirebaseAdmin.Auth;
using System.Security.Cryptography;
using Xamarin.Essentials;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace AccessControlMobileApp.Services
{
    public class UserService
    {
        private UserCredential userAuthCredentials;

        public UserCredential UserAuthCredentials { get => userAuthCredentials; private set => userAuthCredentials = value; }

        private FirebaseAuthClient userAuthClient;
        private FirebaseClient databaseClient;

        public Models.User User { get => user; set => user = value; }

        private Models.User user;

        public UserService()
        {
            var configuration = App.AppConfigService;

            var config = new FirebaseAuthConfig
            {
                ApiKey = configuration.ApiKey,
                AuthDomain = configuration.AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            databaseClient = new FirebaseClient(configuration.WebAddress);
            userAuthClient = new FirebaseAuthClient(config);

            User = new Models.User();
        }

        public async Task RequestUserData()
        {   
            try
            {
                string userId = UserAuthCredentials.User.Uid;
                string path = $"admins/{userId}";
                var snapshot = await databaseClient.Child(path).OnceAsync<object>();
                if (snapshot.Any())
                {
                    ParseDatabase(snapshot);
                    User.IsAdmin = true;
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                string userId = UserAuthCredentials.User.Uid;
                string path = $"users/{userId}";
                var snapshot = await databaseClient.Child(path).OnceAsync<object>();
                if (snapshot.Any())
                {
                    ParseDatabase(snapshot);
                    User.IsAdmin = false;
                } 
            }
            catch (Exception ex)
            {
                
            }
        }

        private void ParseDatabase(IReadOnlyCollection<FirebaseObject<object>> snapshot)
        {
            int i = 0;
            foreach (var item in snapshot)
            {
                switch (i)
                {
                    case 0:
                        User.AccessLevel = Convert.ToInt32(item.Object.ToString());
                        break;
                    case 1:
                        User.Email = item.Object.ToString();
                        break;
                    case 2:
                        User.LastLoginDate = item.Object.ToString();
                        break;
                    case 3:
                        User.PreferedAccessMethod = Convert.ToInt32(item.Object.ToString());
                        break;
                }
                i++;
            }
        }

        public async Task LoginUser(string email, string password)
        {
            try
            {
                UserAuthCredentials = await userAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (FirebaseAuthHttpException ex)
            {
                
            }
        }

        public async Task<string> RegisterUser(string email, string password, bool isAdmin, int accessLevel)
        {
            string result;

            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            if (!Regex.IsMatch(password, passwordPattern))
            {
                return "Password must have a minimum of 8 characters, at least one uppercase letter, one lowercase letter, one digit, and one special symbol.";
            }

            try
            {
                UserAuthCredentials = await userAuthClient.CreateUserWithEmailAndPasswordAsync(email, password);
                result = null;
            }
            catch (FirebaseAuthHttpException ex)
            {
                result =  ParseErrorMessageFromResponse(ex);
            }
            if (result == null)
            {
                string userId = UserAuthCredentials.User.Uid;
                string path;

                if (isAdmin)
                {
                    path = $"admins/{userId}";
                }
                else
                {
                    path = $"users/{userId}";
                }

                var user = new
                {
                    Email = email,
                    AccessLevel = accessLevel,
                    PreferedAccessMethod = 2,
                    LastLoginDate = ""
                };
            
                try
                {
                    await databaseClient.Child(path).PutAsync(user);
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

        public async Task<string> SaveUserSettings(int preferedAccessMethod)
        {
            string result;
            string userId = UserAuthCredentials.User.Uid;
            string path;
            if (user.IsAdmin)
            {
                path = $"admins/{userId}";
            }
            else
            {
                path = $"users/{userId}";
            }
            var updates = new Dictionary<string, object>
            {
                { "PreferedAccessMethod", preferedAccessMethod }  
            };

            try
            {
                await databaseClient.Child(path).PatchAsync(updates);
                user.PreferedAccessMethod = preferedAccessMethod;
                result = null;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public async Task<string> SaveAccountSettings(string oldPassword, string newPassword)
        {
            string result;
            try
            {
                await userAuthClient.SignInWithEmailAndPasswordAsync(user.Email, oldPassword);
                await userAuthClient.User.ChangePasswordAsync(newPassword);
                result = null;
            }
            catch (FirebaseAuthHttpException ex)
            {
                result = ParseErrorMessageFromResponse(ex);
            }
            return result;
        }

        public async Task<string> StoreLastLoginDate()
        {
            string result = null;

            if (result == null)
            {
                string userId = UserAuthCredentials.User.Uid;
                string path;
                if (user.IsAdmin)
                {
                    path = $"admins/{userId}";
                }
                else
                {
                    path = $"users/{userId}";
                }
                string dateTime = DateTime.Now.ToString();
                var updates = new Dictionary<string, object>
                {
                    { "LastLoginDate", dateTime }
                };
                try
                {
                    await databaseClient.Child(path).PatchAsync(updates);
                    result = null;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return result;
        }
    }
}
