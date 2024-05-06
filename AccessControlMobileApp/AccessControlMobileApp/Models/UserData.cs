using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlMobileApp.Models
{
    public class UserData
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessLevel { get; set; }
        public int PreferedAccessMethod { get; set; }

        public UserData(string userId, string email, string username, bool isAdmin, int accessLevel, int preferedAccessMethod)
        { 
            this.UserId = userId;
            this.Email = email;
            this.Username = username;
            this.IsAdmin = isAdmin;
            this.AccessLevel = accessLevel;
            this.PreferedAccessMethod = preferedAccessMethod;
        }

        public UserData()
        { 
        
        }
    }
}
