using AccessControlMobileApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class LogsViewModel : BaseViewModel
    {
        private ObservableCollection<Log> _logs;
        public ObservableCollection<Log> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<UserData> _usersData;
        public ObservableCollection<UserData> UsersData
        {
            get { return _usersData; }
            set
            {
                _usersData = value;
                OnPropertyChanged();
            }
        }

        private bool _isUserListVisible;
        public bool IsUserListVisible
        {
            get { return _isUserListVisible; }
            set
            {
                if (_isUserListVisible != value)
                {
                    _isUserListVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isLogsListVisible;
        public bool IsLogsListVisible
        {
            get { return _isLogsListVisible; }
            set
            {
                if (_isLogsListVisible != value)
                {
                    _isLogsListVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command RequestAllLogsCommand { get; set; }

        public Command RequestAllUsersDataCommand { get; set; }

        public LogsViewModel()
        {
            RequestAllLogsCommand = new Command(async () => await OnRequestAllLogsClicked());
            RequestAllUsersDataCommand = new Command(async () => await OnRequestAllUsersDataClicked());
            IsUserListVisible = true;
            IsLogsListVisible = false;
            OnRequestAllUsersDataClicked();
        }

        public async Task OnRequestAllLogsClicked()
        {
            Logs = new ObservableCollection<Log>(await App.LogsService.RequestAllLogs());
            IsLogsListVisible = true;
            IsUserListVisible = false;
        }

        public async Task OnRequestAllUsersDataClicked()
        {
            UsersData = new ObservableCollection<UserData>(await App.LogsService.RequestAllUsersData());
            IsLogsListVisible = false;
            IsUserListVisible = true;
        }

    }
}
