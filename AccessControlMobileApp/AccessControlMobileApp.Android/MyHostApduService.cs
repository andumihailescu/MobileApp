/*using Android.App;
using Android.Content;
using Android.Nfc.CardEmulators;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccessControlMobileApp.Droid
{
    [Service(Exported = true, Enabled = true)]
    [IntentFilter(new[] { "android.nfc.cardemulation.action.HOST_APDU_SERVICE" },
    Categories = new[] { "android.intent.category.DEFAULT" })]
    [MetaData("android.nfc.cardemulation.host_apdu_service",
         Resource = "@xml/apduservice")]
    public class MyHostApduService : HostApduService
    {
        public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
        {
            var responseApdu;
            // Handle incoming APDU commands
            // ...

            // Return the response APDU
            return responseApdu;
        }

        public override void OnDeactivated(DeactivationReason reason)
        {
            // Handle HCE deactivation
        }
    }
}*/