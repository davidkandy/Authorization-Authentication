using Basics.Models;
using Basics_Advanced.CustomPolicyProvider;
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

        [Authorize(Policy = "Claim.Dob")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRoleAdmin()
        {
            return View("Secret");
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }

        public async Task<IActionResult> InLineAuthorization(
            [FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var authorizationPolicy = builder
                .RequireClaim("Hello")
                .Build();

            var authResult = await authorizationService.AuthorizeAsync(User, authorizationPolicy);
            //var authResult = await authorizationService.AuthorizeAsync(User, "Claim.Dob");

            if (authResult.Succeeded)
            {
                return View("Secret");
            }

            return View("Index");
        }

        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var starrClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "David"),
                new Claim(ClaimTypes.DateOfBirth, "02/28/98"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("Hello", "hii!!"),
                new Claim("Friend", "Good"),
                new Claim(DynamicPolicies.SecurityLevel, "7"),
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