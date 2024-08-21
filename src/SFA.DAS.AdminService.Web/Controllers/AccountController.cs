using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IWebConfiguration _applicationConfiguration;
        private const string ServiceName = "SFA.DAS.AdminService";
        private const string Version = "1.0";

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _applicationConfiguration = ConfigurationService.GetConfig<WebConfiguration>(
                configuration["EnvironmentName"],
                configuration["ConfigurationStorageConnectionString"], 
                Version, 
                ServiceName)
                .Result;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            // Hacking...
            return RedirectToAction(nameof(PostSignIn), "Account");

            //_logger.LogInformation("Start of Sign In");
            //var redirectUrl = Url.Action(nameof(PostSignIn), "Account");

            //// Get the AuthScheme based on the DfESignIn config/property.
            //var authScheme = _applicationConfiguration.UseDfESignIn
            //    ? OpenIdConnectDefaults.AuthenticationScheme
            //    : WsFederationDefaults.AuthenticationScheme;

            //return Challenge(
            //    new AuthenticationProperties { RedirectUri = redirectUrl },
            //    authScheme);
        }

        [HttpGet]
        public IActionResult PostSignIn()
        {
            // Why does the dashboard break when we have a valid role?
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.Surname, "User"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Upn, "test@example.com"),
                new Claim(ClaimTypes.Role, "EPC"),
                new Claim(ClaimTypes.Role, "EPO"),
                new Claim(ClaimTypes.Role, "EPA"),
                new Claim(ClaimTypes.Role, "EPR"),
                new Claim(ClaimTypes.Role, "EPV"),
                new Claim(ClaimTypes.Role, "APR"),
                new Claim(ClaimTypes.Role, "GAC"),
                new Claim(ClaimTypes.Role, "FHC"),
                new Claim(ClaimTypes.Role, "AOV"),
                new Claim(ClaimTypes.Role, "AAC"),
                new Claim(ClaimTypes.Role, "EPX"),
                new Claim(ClaimTypes.Role, "TAD")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

            if (!HttpContext.User.HasValidRole())
            {
                var userName = HttpContext.User.Identity.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == Domain.Roles.RoleClaimType).Select(c => c.Value);
                _logger.LogInformation($"PostSignIn - User '{userName}' does not have a valid role. They have the following roles: '{string.Join(",", roles)}'");

                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                return RedirectToAction("InvalidRole", "Home");
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // Get the AuthScheme based on the DfeSignIn config/property.
            var authScheme = _applicationConfiguration.UseDfESignIn
                ? OpenIdConnectDefaults.AuthenticationScheme
                : WsFederationDefaults.AuthenticationScheme;

            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                authScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            if (HttpContext.User != null)
            {
                var userName = HttpContext.User.Identity.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == Domain.Roles.RoleClaimType).Select(c => c.Value);

                _logger.LogError($"AccessDenied - User '{userName}' does not have a valid role. They have the following roles: '{string.Join(",", roles)}'");
            }

            return View();
        }
    }
}