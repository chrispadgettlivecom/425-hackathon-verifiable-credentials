using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Api.Controllers;
using Shared.Api.Models;
using Shared.Services;

namespace VaccinationCertificateIssuerWebApplication.Api.Controllers
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
            return await IssuanceRequestBase("Kingdom of Zamunda Vaccination Certificate", "KingdomOfZamundaVaccinationCertificate", null);
        }

        [HttpGet("/api/issuer/issuance-response")]
        public ActionResult IssuanceResponse([FromQuery] string state)
        {
            return IssuanceResponseBase(state);
        }
    }
}
