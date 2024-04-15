using AccessControlMobileApp.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database.Query;
using Firebase.Auth;
using Newtonsoft.Json.Linq;

namespace AccessControlMobileApp.Services
{
    public class UserService : Database
    {
        private UserCredential userAuthCredentials;

        public UserCredential UserAuthCredentials { get => userAuthCredentials; private set => userAuthCredentials = value; }

        public UserData UserData { get => userData; set => userData = value; }

        private UserData userData;
        public UserService()
        {
            UserData = new UserData();
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
                    UserData.IsAdmin = true;
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
                    UserData.IsAdmin = false;
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
                        UserData.AccessLevel = Convert.ToInt32(item.Object.ToString());
                        break;
                    case 1:
                        UserData.PreferedAccessMethod = Convert.ToInt32(item.Object.ToString());
                        break;
                    case 2:
                        UserData.Username = item.Object.ToString();
                        break;
                }
                i++;
            }
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
                result = null;
            }
            catch (FirebaseAuthHttpException ex)
            {
                result =  ParseErrorMessageFromResponse(ex);
            }
            if (result == null)
            {
                string userId = UserAuthCredentials.User.Uid;
                string path = $"users/{userId}";
                var user = new
                {
                    Username = username,
                    AccessLevel = 1,
                    PreferedAccessMethod = 1
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

        public async Task<string> SaveUserSettings(string username, int preferedAccessMethod)
        {
            string result;
            string userId = UserAuthCredentials.User.Uid;
            string path;
            if (userData.IsAdmin)
            {
                path = $"admins/{userId}";
            }
            else
            {
                path = $"users/{userId}";
            }
            var updates = new Dictionary<string, object>
            {
                { "Username", username },
                { "PreferedAccessMethod", preferedAccessMethod }  
            };

            try
            {
                await databaseClient.Child(path).PatchAsync(updates);
                userData.PreferedAccessMethod = preferedAccessMethod;
                userData.Username = username;
                result = null;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public async Task<string> SaveAccountSettings(string email, string oldPassword, string newPassword)
        {
            string result;
            try
            {
                await userAuthClient.SignInWithEmailAndPasswordAsync(email, oldPassword);
                await userAuthClient.User.ChangePasswordAsync(newPassword);
                result = null;
            }
            catch (FirebaseAuthHttpException ex)
            {
                result = ParseErrorMessageFromResponse(ex);
            }

            return result;
        }
    }
}
