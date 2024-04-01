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

        public Command LogoutCommand { get; set; }
        public Command RequestAccessCommand { get; set; }

        public Command GoToSettingsCommand { get; set; }

        public RequestAccessViewModel()
        {
            RequestAccessCommand = new Command(OnRequestAccessClicked);
            GoToSettingsCommand = new Command(async () => await GoToSettingsClicked());
            LogoutCommand = new Command(OnLogoutClicked);

            var userService = App.UserService;
            TextBox = TextBox + "/n" + userService.UserAuthCredentials.User;
            TextBox = TextBox + "/n" + userService.UserAuthCredentials.User.Uid;
        }

        public void OnRequestAccessClicked(object obj)
        {
            MessagingCenter.Send<object, bool>(this, "IsActivated", true);
        }

        public async Task GoToSettingsClicked()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new SettingsPage());
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