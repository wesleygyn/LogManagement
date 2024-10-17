using LogManagement.Models.ViewModels;

namespace LogManagement.Services.Interfaces
{
    public interface IUserService
    {
        string GetMyName();
        string GetMyId();

        string GetMacAddress();
        string GetRemoteIPAddress();
        string GetLocalIPv4Address();
        string GetLocalIPv6Address();
        Task<IpApiGeolocationDataViewModel> GetGeolocationFromIP(string ipAddress);
    }
}
