﻿using AccessControlMobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AccessControlMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogDetailsPage : ContentPage
    {
        public LogDetailsPage(object obj)
        {
            InitializeComponent();
            this.BindingContext = new LogDetailsViewModel(obj);
        }
    }
}