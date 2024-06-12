using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlMobileApp.Models
{
    public class Log
    {
        public string UserId { get; set; }
        public string DateAndTime { get; set; }
        public string GateId { get; set; }
        public bool IsAdmin { get; set; }
        public string AccessMethod { get; set; }
        public bool IsApproved { get; set; }
    }
}
