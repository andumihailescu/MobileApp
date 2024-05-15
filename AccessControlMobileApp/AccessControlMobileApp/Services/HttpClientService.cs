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

        private string ipAddress = "192.168.137.215";

        public HttpClientService()
        {
            //DiscoverESP32();
        }
        public async Task<bool> SendMessage(string message)
        {
            try
            {
                string address = "http://" + ipAddress + "/";
                var uri = new Uri(address);
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

        private async Task DiscoverESP32()
        {
            try
            {
                var domains = await ZeroconfResolver.BrowseDomainsAsync();
                var responses = await ZeroconfResolver.ResolveAsync(domains.Select(d => d.Key));

                foreach (var resp in responses)
                {
                    if (resp.DisplayName.Contains("IM414"))
                    {
                        ipAddress = resp.IPAddress;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
