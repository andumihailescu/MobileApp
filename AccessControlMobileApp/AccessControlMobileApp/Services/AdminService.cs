using Firebase.Auth.Requests;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using Xamarin.Essentials;
using AccessControlMobileApp.Models;
using Firebase.Database.Query;

namespace AccessControlMobileApp.Services
{
    public class AdminService : Database
    {
        public AdminService()
        {
            string adminCreditentials = "Add creditentials here";

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(adminCreditentials)
            });
        }


        public async Task<bool> DeleteUser(UserData userData)
        {
            string path;
            if (userData.IsAdmin)
            {
                path = $"/admins/{userData.UserId}";
            }
            else
            {
                path = $"/users/{userData.UserId}";
            }
            try
            {
                await databaseClient.Child(path).DeleteAsync();

                await FirebaseAuth.DefaultInstance.DeleteUserAsync(userData.UserId);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> UpdateUserData(UserData userData, string username, string email, bool isAdmin, int accessLevel)
        {
            string result = null;
            string path;

            if (userData.Email != email)
            {
                UserRecordArgs args = new UserRecordArgs()
                {
                    Uid = userData.UserId,
                    Email = email,
                };
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            }

            if (userData.IsAdmin != isAdmin) 
            { 
                if (isAdmin)
                {
                    path = $"admins/{userData.UserId}";

                    var user = new
                    {
                        Username = username,
                        Email = email,
                        AccessLevel = accessLevel,
                        PreferedAccessMethod = userData.PreferedAccessMethod,
                        FirstTimeLogin = userData.FirstTimeLogin
                    };

                    try
                    {
                        await databaseClient.Child(path).PutAsync(user);
                        string oldPath = $"users/{userData.UserId}";
                        await databaseClient.Child(oldPath).DeleteAsync();
                        result = null;
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
            
            }
            else
            {
                if (isAdmin)
                {
                    path = $"admins/{userData.UserId}";
                }
                else
                {
                    path = $"users/{userData.UserId}";
                }
                var updates = new Dictionary<string, object>
                {
                    { "Username", username },
                    { "Email", email },
                    { "AccessLevel", accessLevel }
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
