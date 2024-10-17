using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Security.Claims;
using LogManagement.Services.Interfaces;
using Newtonsoft.Json;
using LogManagement.Models.ViewModels;

namespace LogManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string IpApiUrl = "http://ip-api.com/json/";

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = this._httpContextAccessor.HttpContext.User.Identities.FirstOrDefault().Name;
            }
            return result;
        }

        public string GetMyId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = this._httpContextAccessor.HttpContext.User.Identities.First().Claims.First().Value;
            }
            return result;
        }
      
        public string GetMacAddress()
        {
            string mac = string.Empty;
            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface nic in networkInterfaces)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        PhysicalAddress macAddress = nic.GetPhysicalAddress();
                        byte[] bytes = macAddress.GetAddressBytes();
                        mac = string.Join(":", bytes.Select(b => b.ToString("X2")));
                        Console.WriteLine("Endereço MAC: " + mac);
                        break; // Mostra apenas o primeiro endereço MAC encontrado.
                    }
                }
            }
            catch (Exception ex)
            {
                mac = "Erro ao obter o endereço MAC: " + ex.Message;
                Console.WriteLine(mac);
            }
            return mac;
        }

        public string GetRemoteIPAddress()
        {
            string ip = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    ip = client.DownloadString("https://api.ipify.org");
                    Console.WriteLine("Endereço IP Público: " + ip);
                }
            }
            catch (Exception ex)
            {
                ip = "Erro ao obter o endereço IP: " + ex.Message;
                Console.WriteLine("Erro ao obter o endereço IP: " + ex.Message);
            }
            return ip;
        }

        public string GetLocalIPv4Address()
        {
            string localIPv4 = null;

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    var properties = networkInterface.GetIPProperties();
                    foreach (var ipAddress in properties.UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            localIPv4 = ipAddress.Address.ToString();
                            break;
                        }
                    }
                }

                if (localIPv4 != null)
                    break;
            }

            return localIPv4;
        }

        public string GetLocalIPv6Address()
        {
            string localIPv6 = null;

            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    var properties = networkInterface.GetIPProperties();
                    foreach (var ipAddress in properties.UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            localIPv6 = ipAddress.Address.ToString();
                            break;
                        }
                    }
                }

                if (localIPv6 != null)
                    break;
            }

            return localIPv6;
        }

        public async Task<IpApiGeolocationDataViewModel> GetGeolocationFromIP(string ipAddress)
        {
            try
            {
                string apiUrl = $"{IpApiUrl}{ipAddress}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        var locationData = JsonConvert.DeserializeObject<IpApiGeolocationDataViewModel>(responseData);

                        return locationData;
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao obter dados de geolocalização. Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter dados de geolocalização: {ex.Message}");

                return null;
            }
            return null;
        }
    }
}