using Firebase.Database.Query;
using System;
using System.Threading.Tasks;
using AccessControlMobileApp.Models;
using System.Linq;
using System.Collections.Generic;

namespace AccessControlMobileApp.Services
{
    public class LogsService : Database
    {
        public LogsService()
        {
            
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
            catch (Exception ex)
            {

            }
            logs.Reverse();
            return logs;
        }

        public async Task<List<UserData>> RequestAllUsersData()
        {
            List<UserData> usersData = new List<UserData>();

            try
            {
                string path = $"admins";
                var snapshot = await databaseClient.Child(path).OnceAsync<UserData>();

                foreach (var childSnapshot in snapshot)
                {
                    UserData userData = new UserData(
                        userId: childSnapshot.Key,
                        email: childSnapshot.Object.Email,
                        isAdmin: true,
                        accessLevel: childSnapshot.Object.AccessLevel,
                        preferedAccessMethod: childSnapshot.Object.PreferedAccessMethod,
                        firstTimeLogin: childSnapshot.Object.FirstTimeLogin
                    );
                    
                    usersData.Add(userData);
                }
                path = $"users";
                snapshot = await databaseClient.Child(path).OnceAsync<UserData>();

                foreach (var childSnapshot in snapshot)
                {
                    UserData userData = new UserData(
                        userId: childSnapshot.Key,
                        email: childSnapshot.Object.Email,
                        isAdmin: false,
                        accessLevel: childSnapshot.Object.AccessLevel,
                        preferedAccessMethod: childSnapshot.Object.PreferedAccessMethod,
                        firstTimeLogin: childSnapshot.Object.FirstTimeLogin
                    );

                    usersData.Add(userData);
                }
            }
            catch (Exception ex)
            {

            }
            return usersData;
        }
    }
}
