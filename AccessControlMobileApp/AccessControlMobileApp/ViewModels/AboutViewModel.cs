using AccessControlMobileApp.Views;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public Command LgtCommand { get; }

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            //LogoutCommand = new Command(async () => await Shell.Current.GoToAsync($"//LoginPage"));
            LgtCommand = new Command(OnLogoutClicked);
        }

        public ICommand OpenWebCommand { get; }

        public ICommand LogoutCommand { get; }

        public async void OnLogoutClicked(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}