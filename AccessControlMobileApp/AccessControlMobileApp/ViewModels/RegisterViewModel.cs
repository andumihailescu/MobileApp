using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
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

        private IList<string> items;
        public IList<string> Items 
        {
            get { return items; }
        }

        private object selectedItem;
        public object SelectedItem { get; set; }
        private int selectedIndex;
        public int SelectedIndex {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command RegisterCommand { get; set; }
        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegisterClicked());
            items = new List<string>
            {
                "0",
                "1",
                "2",
                "3"
            };
        }

        private async Task OnRegisterClicked()
        {
            var userService = App.UserService;
            var result = await userService.RegisterUser(Email, Password, Username, IsAdmin, SelectedIndex);
            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Seccess", "User Registered", "OK");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result, "OK");
            }
        }
    }
}
