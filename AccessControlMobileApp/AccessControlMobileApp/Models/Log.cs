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

        public Log(string userId, string date, string gateId, bool isAdmin, string accessMethod, bool isApproved)
        {
            this.UserId = userId;
            this.DateAndTime = date;
            this.GateId = gateId;
            this.IsAdmin = isAdmin;
            this.AccessMethod = accessMethod;
            this.IsApproved = isApproved;
        }
    }
}
