using AccessControlMobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; }

        private string _username;
        private string _password;
        private string _message;

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                }
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void OnLoginClicked(object obj)
        {
            if ((Username == "UIE99708") && (Password == "1234"))
            {
                Message = "";
                // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
                Username = "";
                Password = "";
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            }
            else
            {
                Message = "Wrong creditentials!";
            }
            
        }
    }
}
