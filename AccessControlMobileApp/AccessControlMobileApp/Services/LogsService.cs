using Firebase.Database.Query;
using System;
using System.Threading.Tasks;
using AccessControlMobileApp.Models;

namespace AccessControlMobileApp.Services
{
    public class LogsService : Database
    {
        public LogsService()
        {
            
        }

        public async Task<string> GenerateLogs()
        {
            var userService = App.UserService;
            string userId = userService.UserAuthCredentials.User.Uid;
            string path = $"z_logs/{userId}";
            string result;
            DateTime dateTime = DateTime.Now;
            string date = dateTime.ToString("dd/MM/yy") + " " + dateTime.ToString("HH:mm:ss");
            var log = new
            {
                Username = userService.UserData.Username,
                AccessLevel = userService.UserData.AccessLevel,
                PreferedAccessMethod = userService.UserData.PreferedAccessMethod,
                IsAdmin = userService.UserData.IsAdmin,
                GateId = 8,
                Date = date,
            };

            try
            {
                await databaseClient.Child(path).PutAsync(log);
                result = null;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
