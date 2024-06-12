using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccessControlMobileApp.Models;
using Firebase.Database.Query;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Firebase.Database;

namespace AccessControlMobileApp.Services
{
    public class AdminService
    {
        protected FirebaseAuthClient userAuthClient;
        protected FirebaseClient databaseClient;

        public AdminService()
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

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(configuration.AdminCreditentials)
            });

            databaseClient = new FirebaseClient(configuration.WebAddress);
            userAuthClient = new FirebaseAuthClient(config);
        }

        private string BuildUserPath(Models.User user)
        {
            string path;
            if (user.IsAdmin)
            {
                path = $"/admins/{user.UserId}";
            }
            else
            {
                path = $"/users/{user.UserId}";
            }
            return path;
        }

        public async Task DeleteUser(Models.User user)
        {
            string path = BuildUserPath(user);
            try
            {
                await databaseClient.Child(path).DeleteAsync();

                await FirebaseAuth.DefaultInstance.DeleteUserAsync(user.UserId);
            }
            catch
            {

            }
        }

        // The User should be updated before calling UpdateUser
        // SRP (Single Responsibility Principle) is violated; this method is doing more than one thing
        // Too many method parameters -> Clean Code
        public async Task UpdateUser(Models.User user, string email, bool isAdmin, int accessLevel)
        {
            string path;

            if (user.Email != email)
            {
                UserRecordArgs args = new UserRecordArgs()
                {
                    Uid = user.UserId,
                    Email = email,
                };
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            }

            if (user.IsAdmin != isAdmin) 
            { 
                if (isAdmin)
                {
                    path = $"admins/{user.UserId}";

                    var newUserData = new
                    {
                        Email = email,
                        AccessLevel = accessLevel,
                        PreferedAccessMethod = user.PreferedAccessMethod,
                        FirstTimeLogin = user.LastLoginDate
                    };

                    try
                    {
                        await databaseClient.Child(path).PutAsync(newUserData);
                        string oldPath = $"users/{user.UserId}";
                        await databaseClient.Child(oldPath).DeleteAsync();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                if (isAdmin)
                {
                    path = $"admins/{user.UserId}";
                }
                else
                {
                    path = $"users/{user.UserId}";
                }
                var updates = new Dictionary<string, object>
                {
                    { "Email", email },
                    { "AccessLevel", accessLevel }
                };

                try
                {
                    await databaseClient.Child(path).PatchAsync(updates);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task<List<Log>> RequestAllLogs()
        {
            List<Log> logs = new List<Log>();

            try
            {
                string path = $"z_logs";
                var snapshot = await databaseClient.Child(path).OnceAsync<Log>();

                foreach (var childSnapshot in snapshot)
                {
                    logs.Add(childSnapshot.Object);
                }
            }
            catch
            {

            }
            logs.Reverse();
            return logs;
        }

        public async Task<List<Models.User>> RequestAllUsers()
        {
            List<Models.User> usersList = new List<Models.User>();

            try
            {
                string path = $"admins";
                var snapshot = await databaseClient.Child(path).OnceAsync<Models.User>();

                foreach (var childSnapshot in snapshot)
                {
                    Models.User user = new Models.User(
                        userId: childSnapshot.Key,
                        email: childSnapshot.Object.Email,
                        isAdmin: true,
                        accessLevel: childSnapshot.Object.AccessLevel,
                        preferedAccessMethod: childSnapshot.Object.PreferedAccessMethod,
                        lastLoginDate: childSnapshot.Object.LastLoginDate
                    );

                    usersList.Add(user);
                }
                path = $"users";
                snapshot = await databaseClient.Child(path).OnceAsync<Models.User>();

                foreach (var childSnapshot in snapshot)
                {
                    Models.User user = new Models.User(
                        userId: childSnapshot.Key,
                        email: childSnapshot.Object.Email,
                        isAdmin: false,
                        accessLevel: childSnapshot.Object.AccessLevel,
                        preferedAccessMethod: childSnapshot.Object.PreferedAccessMethod,
                        lastLoginDate: childSnapshot.Object.LastLoginDate
                    );

                    usersList.Add(user);
                }
            }
            catch (Exception ex)
            {

            }
            return usersList;
        }
    }
}
