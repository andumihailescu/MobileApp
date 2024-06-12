using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AccessControlMobileApp.Services
{
    public class AppConfigService
    {
        private string apiKey;
        private string authDomain;
        private string webAddress;
        private string adminCreditentials;

        public string ApiKey { get => apiKey; private set => apiKey = value; }
        public string AuthDomain { get => authDomain; private set => authDomain = value; }
        public string WebAddress { get => webAddress; private set => webAddress = value; }
        public string AdminCreditentials { get => adminCreditentials; private set => adminCreditentials = value; }

        public AppConfigService()
        {
            string jsonFileName = "appsettings.json";
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");

            using (var reader = new StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();

                JObject jsonObject = JObject.Parse(jsonString);

                ApiKey = jsonObject["ApiKey"].ToString();
                AuthDomain = jsonObject["AuthDomain"].ToString();
                WebAddress = jsonObject["FirebaseUrl"].ToString();
                AdminCreditentials = jsonObject["AdminCreditentials"].ToString();
            }
        }
    }
}
