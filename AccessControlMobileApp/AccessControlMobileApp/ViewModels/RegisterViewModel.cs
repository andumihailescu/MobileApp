using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public  class RegisterViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }
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
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _result;
        public string Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command RegisterCommand { get; set; }
        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegisterClicked());
        }

        private async Task OnRegisterClicked()
        {
            var userService = App.UserService;
            Result = await userService.RegisterUser(Email, Password, Username);
            if (Result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Seccess", "User Registered", "OK");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", Result, "OK");
            }
        }
    }
}
