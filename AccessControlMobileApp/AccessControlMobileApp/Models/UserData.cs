using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlMobileApp.Models
{
    public class UserData
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessLevel { get; set; }
        public int PreferedAccessMethod { get; set; }
    }
}
