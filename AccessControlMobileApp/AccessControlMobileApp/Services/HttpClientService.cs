using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace AccessControlMobileApp.Services
{
    public class HttpClientService
    {
        private static readonly HttpClient client = new HttpClient();

        public HttpClientService()
        {
            
        }
        public async Task SendMessage(string message, string roomId)
        {
            try
            {
                string address = "http://" + roomId + ".local" + "/";
                var uri = new Uri(address);
                var content = new StringContent(message + "\r\n", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = await client.PostAsync(uri, content);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
