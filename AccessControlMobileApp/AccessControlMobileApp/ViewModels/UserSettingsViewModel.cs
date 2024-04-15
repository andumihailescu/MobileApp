﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class UserSettingsViewModel : BaseViewModel
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

        public Command SaveSettings { get; set; }

        public UserSettingsViewModel()
        {
            SaveSettings = new Command(async () => await OnSaveSettingsClicked());
            InitializeSettings();
        }

        public async Task OnSaveSettingsClicked()
        {
            int preferedAccessMethod = 0;
            if (NfcRadioButtonChecked)
            {
                preferedAccessMethod = 0;
            }
            if (BluetoothRadioButtonChecked)
            {
                preferedAccessMethod = 1;
            }
            if (WifiRadioButtonChecked)
            {
                preferedAccessMethod = 2;
            }

            var userService = App.UserService;
            var result = await userService.SaveUserSettings(Username, preferedAccessMethod);
            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Seccess", "Settings Changed", "OK");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result, "OK");
            }
        }

        private void InitializeSettings()
        {
            if (App.UserService.UserData.PreferedAccessMethod == 0)
            {
                NfcRadioButtonChecked = true;
                BluetoothRadioButtonChecked = false;
                WifiRadioButtonChecked = false;
            }
            if (App.UserService.UserData.PreferedAccessMethod == 1)
            {
                NfcRadioButtonChecked = false;
                BluetoothRadioButtonChecked = true;
                WifiRadioButtonChecked = false;
            }
            if (App.UserService.UserData.PreferedAccessMethod == 2)
            {
                NfcRadioButtonChecked = false;
                BluetoothRadioButtonChecked = false;
                WifiRadioButtonChecked = true;
            }

            Username = App.UserService.UserData.Username;
        }
    }
}
