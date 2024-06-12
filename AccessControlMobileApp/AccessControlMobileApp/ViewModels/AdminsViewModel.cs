using AccessControlMobileApp.Models;
using AccessControlMobileApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class AdminsViewModel : BaseViewModel
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

        private ObservableCollection<User> _usersData;
        public ObservableCollection<User> UsersData
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

        public Command GoToRegisterPageCommand { get; set; }

        public Command GoToUserDetailsCommand { get; set; }

        public Command GoToLogDetailsCommand { get; set; }

        public AdminsViewModel()
        {
            RequestAllLogsCommand = new Command(async () => await OnRequestAllLogsClicked());
            RequestAllUsersDataCommand = new Command(async () => await OnRequestAllUsersDataClicked());
            GoToRegisterPageCommand = new Command(async () => await OnGoToRegisterClicked());
            GoToUserDetailsCommand = new Command(async (object sender) => await GoToUserDetails(sender));
            GoToLogDetailsCommand = new Command(async (object sender) => await GoToLogDetails(sender));
            IsUserListVisible = true;
            IsLogsListVisible = false;
            OnRequestAllUsersDataClicked();
        }

        public async Task OnRequestAllLogsClicked()
        {
            Logs = new ObservableCollection<Log>(await App.AdminService.RequestAllLogs());
            IsLogsListVisible = true;
            IsUserListVisible = false;
        }

        public async Task OnRequestAllUsersDataClicked()
        {
            UsersData = new ObservableCollection<User>(await App.AdminService.RequestAllUsers());
            IsLogsListVisible = false;
            IsUserListVisible = true;
        }

        private async Task OnGoToRegisterClicked()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new RegisterPage());
        }

        public async Task GoToUserDetails(object sender)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new UserDetailsPage(sender));
        }

        public async Task GoToLogDetails(object sender)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new LogDetailsPage(sender));
        }
    }
}
