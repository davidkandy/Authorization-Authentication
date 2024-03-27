using Basics.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "ClaimType.Dob")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRoleAdmin()
        {
            return View("Secret");
        }

        public IActionResult Authenticate()
        {
            var starrClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "David"),
                new Claim(ClaimTypes.DateOfBirth, "02/28/98"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("Starr.Says", "DaDa"),
                new Claim(ClaimTypes.Email, "david@dad.com")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Ibegbu David"),
                new Claim("Driving License", "A+")
            };

            var starrIdentities = new ClaimsIdentity(starrClaims, "Starr Identity");
            var licenseIdentities = new ClaimsIdentity(licenseClaims, "Govt");

            var userPrincipal = new ClaimsPrincipal(new[] { starrIdentities, licenseIdentities });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
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
    }
}