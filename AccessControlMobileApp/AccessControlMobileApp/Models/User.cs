using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlMobileApp.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessLevel { get; set; }
        public int PreferedAccessMethod { get; set; }
        public string LastLoginDate { get; set; }

        public User(string userId, string email, bool isAdmin, int accessLevel, int preferedAccessMethod, string lastLoginDate)
        { 
            this.UserId = userId;
            this.Email = email;
            this.IsAdmin = isAdmin;
            this.AccessLevel = accessLevel;
            this.PreferedAccessMethod = preferedAccessMethod;
            this.LastLoginDate = lastLoginDate;
        }

        public User()
        { 
        
        }
    }
}
