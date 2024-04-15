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
        public Command GoToRegisterPageCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLoginClicked());
            GoToRegisterPageCommand = new Command(async () => await OnGoToRegisterClicked());
        }

        private async Task OnLoginClicked()
        {
            var userService = App.UserService;
            var result = await userService.LoginUser(Email, Password);
            if (result == null)
            {
                await App.UserService.RequestUserData();
                Application.Current.MainPage = new RequestAccessPage();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result, "OK");
            }
        }

        private async Task OnGoToRegisterClicked()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new RegisterPage());
        }
    }
}
