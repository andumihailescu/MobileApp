﻿using System;
using Android.Nfc.CardEmulators;
using Android.OS;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Xamarin.Forms;

namespace AccessControlMobileApp.Droid
{
    [Service(Exported = true, Enabled = true, Permission = "android.permission.BIND_NFC_SERVICE"),
        IntentFilter(new[] { "android.nfc.cardemulation.action.HOST_APDU_SERVICE" }, Categories = new[] { "android.intent.category.DEFAULT" }),
        MetaData("android.nfc.cardemulation.host_apdu_service", Resource = "@xml/aid_list")]
    public class HostCardEmulationService : HostApduService
    {

        private const String TAG = "CardService";
        // AID for our loyalty card service.
        private const String SAMPLE_LOYALTY_CARD_AID = "F522222222";

        // ISO-DEP command HEADER for selecting an AID.
        // Format: [Class | Instruction | Parameter 1 | Parameter 2]
        private const String SELECT_APDU_HEADER = "00A40400";

        // "OK" status word sent in response to SELECT AID command (0x9000)
        private static readonly byte[] SELECT_OK_SW = HexStringToByteArray("9000");

        // "UNKNOWN" status word sent in response to invalid APDU command (0x0000)
        private static readonly byte[] UNKNOWN_CMD_SW = HexStringToByteArray("0000");
        private static readonly byte[] SELECT_APDU = BuildSelectApdu(SAMPLE_LOYALTY_CARD_AID);

        private static bool isActivated = false;
        private static string userId = "";

        public override void OnCreate()
        {
            base.OnCreate();
            MessagingCenter.Subscribe<object, bool>(this, "IsActivated", (sender, _isActivated) =>
            {
                isActivated = _isActivated;
            });
            MessagingCenter.Subscribe<object, string>(this, "Userid", (sender, uid) =>
            {
                userId = uid;
            });
        }

        public override void OnDeactivated(DeactivationReason reason)
        {
            
        }

        public static byte[] BuildSelectApdu(string aid)
        {
            // Format: [CLASS | INSTRUCTION | PARAMETER 1 | PARAMETER 2 | LENGTH | DATA]
            return HexStringToByteArray(SELECT_APDU_HEADER + (aid.Length / 2).ToString("X2") + aid);
        }


        public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
        {
            Log.Info(TAG, "Received APDU: " + ByteArrayToHexString(commandApdu));
            // If the APDU matches the SELECT AID command for this service,
            // send the loyalty card account number, followed by a SELECT_OK status trailer (0x9000)

            // Check if the values inside of commandApdu are the same as SELECT_APDU 
            bool arrayEquals;
            if (SELECT_APDU.Length == commandApdu.Length)
            {
                arrayEquals = true;
                for (int i = 0; i < SELECT_APDU.Length; i++)
                {
                    if (SELECT_APDU[i] != commandApdu[i])
                    {
                        arrayEquals = false;
                        break;
                    }
                }
            }
            else
            {
                arrayEquals = false;
            }

            if (arrayEquals)
            {
                if (isActivated)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(userId);
                    isActivated = false;
                    userId = null;
                    return ConcatArrays(messageBytes, SELECT_OK_SW);
                }
                else
                {
                    userId = null;
                    return UNKNOWN_CMD_SW;
                }
            }
            else
            {   
                isActivated = false;
                userId = null;
                return UNKNOWN_CMD_SW;
            }
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            String s = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                s += bytes[i].ToString("X2");
            }
            return s;
        }

        private static byte[] HexStringToByteArray(string s)
        {
            int len = s.Length;
            if (len % 2 == 1)
            {
                throw new ArgumentException("Hex string must have even number of characters");
            }
            byte[] data = new byte[len / 2]; //Allocate 1 byte per 2 hex characters
            for (int i = 0; i < len; i += 2)
            {
                ushort val, val2;
                // Convert each chatacter into an unsigned integer (base-16)
                try
                {
                    val = (ushort)Convert.ToInt32(s[i].ToString() + "0", 16);
                    val2 = (ushort)Convert.ToInt32("0" + s[i + 1].ToString(), 16);
                }
                catch (Exception)
                {
                    continue;
                }

                data[i / 2] = (byte)(val + val2);
            }
            return data;
        }

        public static byte[] ConcatArrays(byte[] first, params byte[][] rest)
        {
            int totalLength = first.Length;
            foreach (byte[] array in rest)
            {
                totalLength += array.Length;
            }
            byte[] result = new byte[totalLength];
            first.CopyTo(result, 0);
            int offset = first.Length;
            foreach (byte[] array in rest)
            {
                array.CopyTo(result, offset);
                offset += array.Length;
            }
            return result;
        }

    }
}