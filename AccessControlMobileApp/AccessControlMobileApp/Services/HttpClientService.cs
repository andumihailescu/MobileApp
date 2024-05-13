using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlMobileApp.Services
{
    public class HttpClientService
    {
        private static readonly HttpClient client = new HttpClient();

        public HttpClientService()
        {

        }
        public async Task<bool> SendMessage(string message)
        {
            try
            { 
                var uri = new Uri("http://192.168.0.105/");
                var content = new StringContent(message + "\r\n", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
