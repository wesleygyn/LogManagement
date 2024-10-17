using LogManagement.Models;
using LogManagement.Models.ViewModels;
using LogManagement.Seeds;
using LogManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;

namespace LogManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IHttpContextAccessor _accessor;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public HomeController(IHttpContextAccessor accessor, ILogger<HomeController> logger, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _accessor = accessor;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        public string LocalIPAddr { get; private set; }

        public async Task <IActionResult> Index()
        {
            IpApiGeolocationDataViewModel geolocationData = await _userService.GetGeolocationFromIP(_userService.GetLocalIPv6Address());

            LoggedUserDataViewModel loggedUserData = new LoggedUserDataViewModel();

            if(geolocationData != null)
            {
                loggedUserData.Query = geolocationData.Query;
                loggedUserData.City = geolocationData.City;
                loggedUserData.RegionName = geolocationData.RegionName;
                loggedUserData.Country = geolocationData.Country;
                loggedUserData.Zip = geolocationData.Zip;
                loggedUserData.Timezone = geolocationData.Timezone;
                loggedUserData.Lat = geolocationData.Lat;
                loggedUserData.Lon = geolocationData.Lon;
                loggedUserData.DeviceName = System.Net.Dns.GetHostName();
                loggedUserData.IpRemote = _userService.GetRemoteIPAddress();
                loggedUserData.IPv4Local = _userService.GetLocalIPv4Address();
                loggedUserData.IPv6Local = _userService.GetLocalIPv6Address();
                loggedUserData.MacAddress = _userService.GetMacAddress(); ;
                loggedUserData.UserID = _userService.GetMyId();
                loggedUserData.UserName = _userService.GetMyName();
            }

            CookieManager();
            return View(loggedUserData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void CookieManager()
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value).ToList();

            Response.Cookies.Delete("Permission");
            Response.Cookies.Append("Permission", string.Join(",", roles));
        }
    }
}