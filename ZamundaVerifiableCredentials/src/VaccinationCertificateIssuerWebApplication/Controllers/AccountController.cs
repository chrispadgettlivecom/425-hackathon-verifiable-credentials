using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace VaccinationCertificateIssuerWebApplication.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> LoggedIn(string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

            if (authenticateResult == null || !authenticateResult.Succeeded)
            {
                return Unauthorized();
            }

            var openIdConnectPrincipal = authenticateResult.Principal;
            var applicationIdentity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

            foreach (var openIdConnectIssuerClaim in openIdConnectPrincipal.Claims.Where(
                claim => claim.Type == ClaimTypes.NameIdentifier ||
                         claim.Type == ClaimTypes.GivenName ||
                         claim.Type == ClaimTypes.Surname))
            {
                var applicationIssuerClaim = new Claim(openIdConnectIssuerClaim.Type, openIdConnectIssuerClaim.Value);
                applicationIdentity.AddClaim(applicationIssuerClaim);
            }

            var applicationIssuerBirthdateClaim = new Claim("birthdate", "1980-01-11");
            applicationIdentity.AddClaim(applicationIssuerBirthdateClaim);
            var applicationIssuerPersonalNumberClaim = new Claim("personal_number", "4901224131");
            applicationIdentity.AddClaim(applicationIssuerPersonalNumberClaim);
            var applicationIssuerVaccineNameClaim = new Claim("vaccine_name", "BNT162b2");
            applicationIdentity.AddClaim(applicationIssuerVaccineNameClaim);
            var applicationIssuerDateOfVaccinationClaim = new Claim("date_of_vaccination", "2021-09-22");
            applicationIdentity.AddClaim(applicationIssuerDateOfVaccinationClaim);

            var applicationPrincipal = new ClaimsPrincipal(applicationIdentity);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, applicationPrincipal);

            if (!Url.IsLocalUrl(returnUrl))
            {
                return Forbid(
                    new AuthenticationProperties(
                        new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.AccessDenied,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Access is denied."
                        }),
                    IdentityConstants.ApplicationScheme);
            }

            return Redirect(returnUrl);
        }

        public IActionResult LogIn(string returnUrl)
        {
            var challengeProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(
                    "LoggedIn",
                    new
                    {
                        returnUrl
                    })
            };

            return Challenge(challengeProperties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Registration()
        {
            return View();
        }
    }
}
