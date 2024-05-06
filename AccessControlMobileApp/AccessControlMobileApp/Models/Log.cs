using System;
using System.Collections.Generic;
using System.Text;

namespace AccessControlMobileApp.Models
{
    public class Log
    {
        public string UserId { get; set; }
        public string Date { get; set; }
        public int GateId { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessMethod { get; set; }
        public bool IsApproved { get; set; }

        public Log(string userId, string date, int gateId, bool isAdmin, int accessMethod, bool isApproved)
        {
            this.UserId = userId;
            this.Date = date;
            this.GateId = gateId;
            this.IsAdmin = isAdmin;
            this.AccessMethod = accessMethod;
            this.IsApproved = isApproved;
        }
    }
}
