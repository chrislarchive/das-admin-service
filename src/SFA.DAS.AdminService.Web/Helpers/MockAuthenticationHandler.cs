using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.AdminService.Web.Domain;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Helpers
{
    //public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    //{
    //    private const string GivenName = "Test";
    //    private const string Surname = "User";
    //    private const string Email = "Test.User@example.com";
    //    private const string RoleClaimType = "http://service/service";

    //    public MockAuthenticationHandler(
    //        IOptionsMonitor<AuthenticationSchemeOptions> options,
    //        ILoggerFactory logger,
    //        UrlEncoder encoder,
    //        ISystemClock clock)
    //        : base(options, logger, encoder, clock)
    //    {
    //    }

    //    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    //    {
    //        // Does this even do anything? I don't think so... ? 
    //        var claims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.GivenName, GivenName),
    //            new Claim(ClaimTypes.Surname, Surname),
    //            new Claim(ClaimTypes.Name, $"{GivenName} {Surname}"),
    //            new Claim(ClaimTypes.Email, Email),
    //            new Claim(ClaimTypes.Upn, Email),
    //            new Claim(ClaimTypes.Role, RoleClaimType)
    //        };

    //        var identity = new ClaimsIdentity(claims, "Mock");
    //        var principal = new ClaimsPrincipal(identity);
    //        var ticket = new AuthenticationTicket(principal, "Mock");

    //        return Task.FromResult(AuthenticateResult.Success(ticket));
    //    }
    //}
    public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MockAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, "Test"),
            new Claim(ClaimTypes.Surname, "User"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Upn, "test@example.com"),
            new Claim(ClaimTypes.Role, "GAC")
        };

            var identity = new ClaimsIdentity(claims, "Mock");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Mock");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

}