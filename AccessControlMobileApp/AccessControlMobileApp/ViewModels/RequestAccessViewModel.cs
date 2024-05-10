using AccessControlMobileApp.Models;
using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class RequestAccessViewModel : BaseViewModel
    {
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
        private int _preferedAccessMethod;
        public int PreferedAccessMethod
        {
            get { return _preferedAccessMethod; }
            set
            {
                if (_preferedAccessMethod != value)
                {
                    _preferedAccessMethod = value;
                    if (_preferedAccessMethod == 1)
                    {
                        BluetoothAccess = true;
                    }
                    else
                    {
                        BluetoothAccess = false;
                    }
                    OnPropertyChanged();
                }
            }
        }
        private bool _bluetoothAccess;
        public bool BluetoothAccess
        {
            get { return _bluetoothAccess; }
            set
            {
                if (_bluetoothAccess != value)
                {
                    _bluetoothAccess = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<object> _availableDevices;
        public ObservableCollection<object> AvailableDevices
        {
            get { return _availableDevices; }
            set
            {
                _availableDevices = value;
                OnPropertyChanged();
            }
        }

        private UserService userService;

        public Command LogoutCommand { get; set; }
        public Command RequestAccessCommand { get; set; }
        public Command GoToUserSettingsCommand { get; set; }
        public Command GoToAccountSettingsCommand { get; set; }
        public Command GoToAdminsPageCommand { get; set; }
        public Command ScanButtonClicked { get; set; }
        public Command SelectBluetoothDevice { get; set; }

        public RequestAccessViewModel()
        {
            RequestAccessCommand = new Command(async () => await OnRequestAccessClicked());
            GoToUserSettingsCommand = new Command(async () => await GoToUserSettingsClicked());
            GoToAccountSettingsCommand = new Command(GoToAccountSettingsClicked);
            LogoutCommand = new Command(OnLogoutClicked);
            GoToAdminsPageCommand = new Command(async () => await GoToAdminsPage());
            ScanButtonClicked = new Command(async () => await OnScanButtonClicked());
            SelectBluetoothDevice = new Command(async (object device) => await OnSelectBluetoothDevice(device));

            HasAdminRights = App.UserService.UserData.IsAdmin;
            userService = App.UserService;
            PreferedAccessMethod = userService.UserData.PreferedAccessMethod;
        }

        public async Task OnRequestAccessClicked()
        {
            if (PreferedAccessMethod == 0)
            {
                MessagingCenter.Send<object, string>(this, "Userid", userService.UserAuthCredentials.User.Uid);
                MessagingCenter.Send<object, bool>(this, "IsActivated", true);
                await App.LogsService.GenerateLogs();
            }
            if (PreferedAccessMethod == 1)
            {
                await App.BluetoothService.SendMessage(userService.UserAuthCredentials.User.Uid);
                await App.LogsService.GenerateLogs();
            }
            if (PreferedAccessMethod == 2)
            {
                await App.HttpClientService.SendMessage(userService.UserAuthCredentials.User.Uid);
                await App.LogsService.GenerateLogs();
            }
        }

        public async Task GoToUserSettingsClicked()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new UserSettingsPage());
        }

        public void GoToAccountSettingsClicked()
        {
            Application.Current.MainPage = new AccountSettingsPage();
        }

        public void OnLogoutClicked()
        {
            MessagingCenter.Send<object, bool>(this, "IsActivated", false);
            var userService = App.UserService;
            userService.LogoutUser();
            Application.Current.MainPage = new LoginPage();
        }

        public async Task GoToAdminsPage()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AdminsPage());
        }

        private async Task<bool> PermissionsGrantedAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        private async Task OnScanButtonClicked()
        {
            IsLoading = true;

            if (!await PermissionsGrantedAsync())                                                   
            {
                await Application.Current.MainPage.DisplayAlert("Permission required", "Application needs location permission", "OK");
                IsLoading = false;
                return;
            }

            AvailableDevices = new ObservableCollection<object>(await App.BluetoothService.ScanForDevices());
            IsLoading = false;
        }

        public async Task OnSelectBluetoothDevice(object device)
        {
            IsLoading = true;

            try
            {
                await App.BluetoothService.ConnectToDevice(device);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error connecting", $"Error connecting to BLE device: {device.ToString() ?? "N/A"}", "Retry");
            }


            IsLoading = false;
        }
    }
}