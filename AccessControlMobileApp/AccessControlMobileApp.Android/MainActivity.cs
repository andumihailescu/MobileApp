using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using System.Linq;

namespace AccessControlMobileApp.Droid
{
    [Activity(Label = "AccessControlMobileApp", 
        Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [Obsolete]
        public override void OnBackPressed()
        {
            bool isNavigationStackEmpty = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count == 0;
            bool isModalStackEmpty = Xamarin.Forms.Application.Current.MainPage.Navigation.ModalStack.Count == 0;
            if (isModalStackEmpty)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string action = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet("Exit the app?", "Cancel", "Yes");
                    switch (action)
                    {
                        case "Cancel":
                            break;
                        case "Yes":
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                            break;
                        default:
                            break;
                    }
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
                });
            }
        }
    }
}