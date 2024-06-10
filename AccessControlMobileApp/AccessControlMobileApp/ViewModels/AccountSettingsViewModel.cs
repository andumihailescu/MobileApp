using AccessControlMobileApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class AccountSettingsViewModel : BaseViewModel
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

        private string _oldPassword;
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                if (_oldPassword != value)
                {
                    _oldPassword = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command SaveSettings { get; set; }

        public AccountSettingsViewModel()
        {
            SaveSettings = new Command(async () => await OnSaveSettingsClicked());
        }
        public async Task OnSaveSettingsClicked()
        {
            var userService = App.UserService;
            var result = await userService.SaveAccountSettings(OldPassword, NewPassword);
            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Seccess", "Password Changed", "OK");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result, "OK");
            }
        }
    }
}
