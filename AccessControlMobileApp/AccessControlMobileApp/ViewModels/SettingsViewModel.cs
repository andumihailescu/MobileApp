using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
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

        public SettingsViewModel()
        {

        }

    }
}