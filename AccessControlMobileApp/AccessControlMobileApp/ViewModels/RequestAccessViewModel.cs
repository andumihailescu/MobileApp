using AccessControlMobileApp.Models;
using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class RequestAccessViewModel : BaseViewModel
    {
        
        private string _textBox;
        public string TextBox
        {
            get { return _textBox; }
            set
            {
                if (_textBox != value)
                {
                    _textBox = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasAdminRights;

        public bool HasAdminRights
        {
            get { return _hasAdminRights; }
            set
            {
                if (_hasAdminRights != value)
                {
                    _hasAdminRights = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command LogoutCommand { get; set; }
        public Command RequestAccessCommand { get; set; }

        public Command GoToUserSettingsCommand { get; set; }
        public Command GoToAccountSettingsCommand { get; set; }

        public RequestAccessViewModel()
        {
            RequestAccessCommand = new Command(async () => await OnRequestAccessClicked());
            GoToUserSettingsCommand = new Command(async () => await GoToUserSettingsClicked());
            GoToAccountSettingsCommand = new Command(async () => await GoToAccountSettingsClicked());
            LogoutCommand = new Command(OnLogoutClicked);

            HasAdminRights = App.UserService.UserData.IsAdmin;
        }

        public async Task OnRequestAccessClicked()
        {
            var userService = App.UserService;
            int preferedAccessMethod = userService.UserData.PreferedAccessMethod;
            if (preferedAccessMethod == 0)
            {
                MessagingCenter.Send<object, string>(this, "Userid", userService.UserAuthCredentials.User.Uid);
                MessagingCenter.Send<object, bool>(this, "IsActivated", true);
                await App.LogsService.GenerateLogs();

            }
            if (preferedAccessMethod == 1)
            {
                //implement Bluetooth
                await App.LogsService.GenerateLogs();
            }
            if (preferedAccessMethod == 2)
            {
                //implement Wi-Fi
                await App.LogsService.GenerateLogs();
            }
        }

        public async Task GoToUserSettingsClicked()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new UserSettingsPage());
        }

        public async Task GoToAccountSettingsClicked()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AccountSettingsPage());
        }

        public void OnLogoutClicked()
        {
            MessagingCenter.Send<object, bool>(this, "IsActivated", false);
            var userService = App.UserService;
            userService.LogoutUser();
            Application.Current.MainPage = new LoginPage();
        }
    }
}