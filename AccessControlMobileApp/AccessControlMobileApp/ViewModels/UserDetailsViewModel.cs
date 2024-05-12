using AccessControlMobileApp.Models;
using AccessControlMobileApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AccessControlMobileApp.ViewModels
{
    public class UserDetailsViewModel : BaseViewModel
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
        private int _accessLevel;
        public int AccessLevel
        {
            get { return _accessLevel; }
            set
            {
                if (_accessLevel != value)
                {
                    _accessLevel = value;
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
                    OnPropertyChanged();
                }
            }
        }
        private string _btnText;
        public string BtnText
        {
            get { return _btnText; }
            set
            {
                if (_btnText != value)
                {
                    _btnText = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isInDisplayMode;
        public bool IsInDisplayMode
        {
            get { return _isInDisplayMode; }
            set
            {
                if (_isInDisplayMode != value)
                {
                    _isInDisplayMode = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isInEditingMode;
        public bool IsInEditingMode
        {
            get { return _isInEditingMode; }
            set
            {
                if (_isInEditingMode != value)
                {
                    _isInEditingMode = value;
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
        public int SelectedIndex
        {
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

        private UserData userData;

        public Command OnManageUserCommand { get; set; }
        public Command OnDeleteUserAccountCommand { get; set; }
        
        public UserDetailsViewModel(object obj)
        {
            OnManageUserCommand = new Command(async () => await ManageUser());
            OnDeleteUserAccountCommand = new Command(async () => await DeleteUserAccount());
            SelectedItemChangedEventArgs user = (SelectedItemChangedEventArgs)obj;
            userData = (UserData)user.SelectedItem;

            Username = userData.Username;
            Email = userData.Email;
            IsAdmin = userData.IsAdmin;
            AccessLevel = userData.AccessLevel;
            PreferedAccessMethod = userData.PreferedAccessMethod;

            BtnText = "Edit User Data";
            IsInEditingMode = false;
            IsInDisplayMode = true;

            items = new List<string>
            {
                "0",
                "1",
                "2",
                "3"
            };
            SelectedIndex = AccessLevel;
        }

        public async Task ManageUser()
        {
            if (_isInEditingMode == false)
            {
                BtnText = "Save User Data";
                IsInEditingMode = true;
                IsInDisplayMode = false;
            }
            else
            {
                AccessLevel = SelectedIndex;
                await App.AdminService.UpdateUserData(userData, Username, Email, IsAdmin, AccessLevel);
                BtnText = "Edit User Data";
                IsInEditingMode = false;
                IsInDisplayMode = true;
            }
            
        }

        public async Task DeleteUserAccount()
        {
            await App.AdminService.DeleteUser(userData);
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
