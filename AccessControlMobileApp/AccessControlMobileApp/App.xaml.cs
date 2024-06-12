using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;

using Microsoft.Extensions.Configuration;

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AccessControlMobileApp
{
    public partial class App : Application
    {
        public static UserService UserService { get; private set; }
        public static AppConfigService AppConfigService { get; private set; }
        public static BluetoothService BluetoothService { get; private set; }
        public static HttpClientService HttpClientService { get; private set; }
        public static AdminService AdminService { get; private set; }

        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            /*var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            var apiKey = config["ApiKey"];
            var authDomain = config["AuthDomain"];
            var firebaseUrl = config["FirebaseUrl"];*/

            AppConfigService = new AppConfigService();
            UserService = new UserService();
            AdminService = new AdminService();
            BluetoothService = new BluetoothService();
            HttpClientService = new HttpClientService();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
