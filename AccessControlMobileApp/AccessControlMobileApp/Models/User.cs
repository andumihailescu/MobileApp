using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlMobileApp.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessLevel { get; set; }
        public int PrefferedAccessMethod { get; set; }
        
        
    }
}
