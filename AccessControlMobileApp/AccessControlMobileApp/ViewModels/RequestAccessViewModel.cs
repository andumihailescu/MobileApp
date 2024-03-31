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
        

        private bool _nfcRadioButtonChecked;
        public bool NfcRadioButtonChecked
        {
            get { return _nfcRadioButtonChecked; }
            set
            {
                if (_nfcRadioButtonChecked != value)
                {
                    _nfcRadioButtonChecked = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _bluetoothRadioButtonChecked;
        public bool BluetoothRadioButtonChecked
        {
            get { return _bluetoothRadioButtonChecked; }
            set
            {
                if (_bluetoothRadioButtonChecked != value)
                {
                    _bluetoothRadioButtonChecked = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _wifiRadioButtonChecked;
        public bool WifiRadioButtonChecked
        {
            get { return _wifiRadioButtonChecked; }
            set
            {
                if (_wifiRadioButtonChecked != value)
                {
                    _wifiRadioButtonChecked = value;
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
            Application.Current.MainPage = new LoginPage();
        }
    }
}