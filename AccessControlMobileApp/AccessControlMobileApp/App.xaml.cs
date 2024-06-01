﻿using AccessControlMobileApp.Services;
using AccessControlMobileApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AccessControlMobileApp
{
    public partial class App : Application
    {
        public static UserService UserService { get; private set; }
        public static LogsService LogsService { get; private set; }
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
            UserService = new UserService();
            LogsService = new LogsService();
            BluetoothService = new BluetoothService();
            HttpClientService = new HttpClientService();
            AdminService = new AdminService();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
