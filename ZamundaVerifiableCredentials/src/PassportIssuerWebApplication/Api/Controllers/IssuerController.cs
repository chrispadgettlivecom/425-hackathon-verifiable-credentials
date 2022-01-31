using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Api.Controllers;
using Shared.Api.Models;
using Shared.Services;

namespace PassportIssuerWebApplication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IssuerController : IssuerControllerBase
    {
        public IssuerController(IOptions<AppSettingsModel> appSettingsAccessor, IMemoryCache memoryCache, IVerifiableCredentialsRequestService requestService)
            : base(appSettingsAccessor, memoryCache, requestService)
        {
        }

        [HttpPost("/api/issuer/issuance-callback")]
        public ActionResult IssuanceCallback(IssuanceCallbackApiRequestModel apiRequestModel)
        {
            return IssuanceCallbackBase(apiRequestModel);
        }

        [HttpGet("/api/issuer/issuance-request")]
        public async Task<ActionResult> IssuanceRequest()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

            if (authenticateResult == null || !authenticateResult.Succeeded)
            {
                return Unauthorized();
            }

            var principal = authenticateResult.Principal;
            var issuanceClaims = new Dictionary<string, string>();
            var givenNameClaimValue = principal.Claims.Where(claim => claim.Type == ClaimTypes.GivenName).Select(claim => claim.Value).Single();
            issuanceClaims.Add("given_name", givenNameClaimValue);
            var familyNameClaimValue = principal.Claims.Where(claim => claim.Type == ClaimTypes.Surname).Select(claim => claim.Value).Single();
            issuanceClaims.Add("family_name", familyNameClaimValue);
            issuanceClaims.Add("birthdate", "1980-01-11");
            issuanceClaims.Add("document_number", "83774554");
            issuanceClaims.Add("date_of_issuance", "2021-04-20");
            issuanceClaims.Add("date_of_expiry", "2031-04-19");
            return await IssuanceRequestBase("Kingdom of Zamunda Passport", "KingdomOfZamundaPassport", issuanceClaims);
        }

        [HttpGet("/api/issuer/issuance-response")]
        public ActionResult IssuanceResponse([FromQuery] string state)
        {
            return IssuanceResponseBase(state);
        }
    }
}
