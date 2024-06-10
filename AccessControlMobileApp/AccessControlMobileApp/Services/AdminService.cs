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

        // We should not have a bool here; replace Task<bool> with Task. 
        // If something gets wrong, an exception should be thrown
        // Replace UserData with User
        public async Task<bool> DeleteUser(UserData userData)
        {
            // Extract path building in a separate private method
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

        // Replace UserData with User
        // Replace UpdateUserData with UpdateUser
        // We should not have a string here; replace Task<string> with Task
        // The User should be updated before calling UpdateUser
        // SRP (Single Responsibility Principle) is violated; this method is doing more than one thing
        // Too many method parameters -> Clean Code
        public async Task<string> UpdateUserData(UserData userData, string email, bool isAdmin, int accessLevel)
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
                // Duplicated code -> Code smell
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
