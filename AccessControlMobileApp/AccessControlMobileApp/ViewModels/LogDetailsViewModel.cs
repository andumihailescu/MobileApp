using AccessControlMobileApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class LogDetailsViewModel : BaseViewModel
    {
        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isAdmin;
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _accessMethod;
        public string AccessMethod
        {
            get { return _accessMethod; }
            set
            {
                if (_accessMethod != value)
                {
                    _accessMethod = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _requestStatus;
        public string RequestStatus
        {
            get { return _requestStatus; }
            set
            {
                if (_requestStatus != value)
                {
                    _requestStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _gateId;
        public string GateId
        {
            get { return _gateId; }
            set
            {
                if (_gateId != value)
                {
                    _gateId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Log logData;

        public LogDetailsViewModel(object obj)
        {
            SelectedItemChangedEventArgs log = (SelectedItemChangedEventArgs)obj;
            logData = (Log)log.SelectedItem;

            UserId = logData.UserId;
            Date = logData.DateAndTime;
            IsAdmin = logData.IsAdmin;
            AccessMethod = logData.AccessMethod;
            if (logData.IsApproved)
            {
                RequestStatus = "Approved";
            }
            else
            {
                RequestStatus = "Declined";
            }
            GateId = logData.GateId;
        }
    }
}
