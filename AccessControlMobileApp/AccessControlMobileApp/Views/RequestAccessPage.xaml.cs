using AccessControlMobileApp.ViewModels;
using Xamarin.Forms;

namespace AccessControlMobileApp.Views
{
    public partial class RequestAccessPage : ContentPage
    {
        public RequestAccessPage()
        {
            InitializeComponent();
        }

        /*protected override async void OnAppearing()
        {
            base.OnAppearing();

            //To be modified here
            if (App.UserService.UserData.PreferedAccessMethod == 1)
            {
                await App.BluetoothService.ScanForDevices();
                await App.BluetoothService.ConnectToDevice("AccessControlSystem");
            }
        }*/

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (BindingContext is RequestAccessViewModel viewModel)
            {
                if (viewModel.SelectBluetoothDevice.CanExecute(e.Item))
                {
                    viewModel.SelectBluetoothDevice.Execute(e.Item);
                }
            }
        }
    }
}