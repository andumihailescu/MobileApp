using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLoginClicked());
        }

        private async Task OnLoginClicked()
        {
            var userService = App.UserService;
            var result = await userService.LoginUser(Email, Password);
            if (result == null)
            {
                await App.UserService.RequestUserData();
                await App.UserService.StoreLastLoginDate();
                if (userService.User.LastLoginDate.Length == 0)
                {
                    Password = "";
                    await Application.Current.MainPage.Navigation.PushModalAsync(new AccountSettingsPage());
                }
                else
                {
                    Application.Current.MainPage = new RequestAccessPage();
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result, "OK");
            }
        }
    }
}
