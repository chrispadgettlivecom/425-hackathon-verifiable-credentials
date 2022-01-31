using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace VaccinationCertificateIssuerWebApplication.Controllers
{
    public class OAuth2Controller : Controller
    {
        [HttpGet("~/oauth2/authorize")]
        [HttpPost("~/oauth2/authorize")]
        public async Task<ActionResult> Authorize()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (authenticateResult == null || !authenticateResult.Succeeded)
            {
                var authenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = $"{Request.PathBase}{Request.Path}{QueryString.Create(Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())}"
                };

                return Challenge(authenticationProperties, IdentityConstants.ApplicationScheme);
            }

            var applicationPrincipal = authenticateResult.Principal;
            var tokenIdentity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);
            var applicationSubjectClaimValue = applicationPrincipal.GetClaim(ClaimTypes.NameIdentifier);

            using (var md5 = MD5.Create())
            {
                var tokenSubjectClaim = new Claim("sub", Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(applicationSubjectClaimValue))));
                tokenIdentity.AddClaim(tokenSubjectClaim);
            }

            var applicationGivenNameClaimValue = applicationPrincipal.GetClaim(ClaimTypes.GivenName);
            var tokenGivenNameClaim = new Claim("given_name", applicationGivenNameClaimValue);
            tokenIdentity.AddClaim(tokenGivenNameClaim);
            var applicationFamilyNameClaimValue = applicationPrincipal.GetClaim(ClaimTypes.Surname);
            var tokenFamilyNameClaim = new Claim("family_name", applicationFamilyNameClaimValue);
            tokenIdentity.AddClaim(tokenFamilyNameClaim);
            var applicationBirthdateClaimValue = applicationPrincipal.GetClaim("birthdate");
            var tokenBirthdateClaim = new Claim("birthdate", applicationBirthdateClaimValue);
            tokenIdentity.AddClaim(tokenBirthdateClaim);
            var applicationPersonalNumberClaimValue = applicationPrincipal.GetClaim("personal_number");
            var tokenPersonalNumberClaim = new Claim("personal_number", applicationPersonalNumberClaimValue);
            tokenIdentity.AddClaim(tokenPersonalNumberClaim);
            var applicationVaccineNameClaimValue = applicationPrincipal.GetClaim("vaccine_name");
            var tokenVaccineNameClaim = new Claim("vaccine_name", applicationVaccineNameClaimValue);
            tokenIdentity.AddClaim(tokenVaccineNameClaim);
            var applicationDateOfVaccinationClaimValue = applicationPrincipal.GetClaim("date_of_vaccination");
            var tokenDateOfVaccinationClaim = new Claim("date_of_vaccination", applicationDateOfVaccinationClaimValue);
            tokenIdentity.AddClaim(tokenDateOfVaccinationClaim);

            foreach (var tokenClaim in tokenIdentity.Claims)
            {
                tokenClaim.SetDestinations(OpenIddictConstants.Destinations.IdentityToken, OpenIddictConstants.Destinations.AccessToken);
            }

            var tokenPrincipal = new ClaimsPrincipal(tokenIdentity);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return SignIn(tokenPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
