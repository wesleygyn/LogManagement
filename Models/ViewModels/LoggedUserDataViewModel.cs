using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace LogManagement.Models.ViewModels
{
    public class LoggedUserDataViewModel
    {
        [DisplayName("IP da Internet")]
        public string Query { get; set; }
        [DisplayName("Cidade")]
        public string City { get; set; }
        [DisplayName("Estado")]
        public string RegionName { get; set; }
        [DisplayName("País")]
        public string Country { get; set; }
        [DisplayName("CEP")]
        public string Zip { get; set; }
        [DisplayName("Fuso horário")]
        public string Timezone { get; set; }
        [DisplayName("Latitude")]
        public string Lat { get; set; }
        [DisplayName("Longitude")]
        public string Lon { get; set; }
        [DisplayName("Nome do dispositivo")]
        public string DeviceName { get; set; }
        [DisplayName("IP da Internet")]
        public string IpRemote { get; set; }
        [DisplayName("IPV4")]
        public string IPv4Local { get; set; }
        [DisplayName("IPV6")]
        public string IPv6Local { get; set; }
        [DisplayName("Endereço Mac")]
        public string MacAddress { get; set; }
        [DisplayName("Id do Usuário")]
        public string UserID { get; set; }
        [DisplayName("E-mail do Usuário")]
        public string UserName { get; set; }
    }
}