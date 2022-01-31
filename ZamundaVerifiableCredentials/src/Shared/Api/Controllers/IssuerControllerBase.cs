using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Api.Models;
using Shared.Services;

namespace Shared.Api.Controllers
{
    //[ApiController]
    //[Route("api/[controller]/[action]")]
    public abstract class IssuerControllerBase : ControllerBase
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IMemoryCache _cache;
        private readonly IVerifiableCredentialsRequestService _requestService;

        protected IssuerControllerBase(IOptions<AppSettingsModel> appSettingsAccessor, IMemoryCache memoryCache, IVerifiableCredentialsRequestService requestService)
        {
            if (appSettingsAccessor == null)
            {
                throw new ArgumentNullException(nameof(appSettingsAccessor));
            }

            _appSettings = appSettingsAccessor.Value;
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        }

        //[HttpPost("/api/issuer/issuance-callback")]
        protected ActionResult IssuanceCallbackBase(IssuanceCallbackApiRequestModel apiRequestModel)
        {
            if (apiRequestModel == null)
            {
                throw new ArgumentNullException(nameof(apiRequestModel));
            }

            if (apiRequestModel.Code == "request_retrieved")
            {
                var cacheData = new
                {
                    code = apiRequestModel.Code,
                    message = "QR code scanned. Waiting for issuance..."
                };

                _cache.Set(apiRequestModel.State, JsonConvert.SerializeObject(cacheData));
            }

            if (apiRequestModel.Code == "issuance_successful")
            {
                var cacheData = new
                {
                    code = apiRequestModel.Code,
                    message = "Credential issued. Success!"
                };

                _cache.Set(apiRequestModel.State, JsonConvert.SerializeObject(cacheData));
            }

            if (apiRequestModel.Code == "issuance_error")
            {
                var cacheData = new
                {
                    code = apiRequestModel.Code,
                    message = $"{apiRequestModel.Error.Code}: {apiRequestModel.Error.Message}"
                };

                _cache.Set(apiRequestModel.State, JsonConvert.SerializeObject(cacheData));
            }

            return Ok();
        }

        //[HttpGet("/api/issuer/issuance-request")]
        protected async Task<ActionResult> IssuanceRequestBase(string issuerName, string issuanceType, IDictionary<string, string> issuanceClaims)
        {
            //var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

            //if (authenticateResult == null || !authenticateResult.Succeeded)
            //{
            //    return Unauthorized();
            //}

            //var principal = authenticateResult.Principal;

            var serviceRequestModel = new IssuanceRequestServiceRequestModel
            {
                IncludeQrCode = false,
                Authority = _appSettings.IssuerAuthority,
                Registration = new RequestRegistrationModel
                {
                    ClientName = issuerName//"Kingdom of Zamunda Passport"
                },
                Issuance = new RequestIssuanceModel
                {
                    Type = issuanceType,//"KingdomOfZamundaPassport",
                    Manifest = _appSettings.CredentialManifest
                },
                Callback = new CallbackModel
                {
                    Url = $"{GetRequestBaseUrl()}/api/issuer/issuance-callback",
                    State = Guid.NewGuid().ToString()
                }
            };

            //serviceRequestModel.Issuance.Claims = new Dictionary<string, string>();
            //var givenNameClaimValue = principal.Claims.Where(claim => claim.Type == ClaimTypes.GivenName).Select(claim => claim.Value).Single();
            //serviceRequestModel.Issuance.Claims.Add("given_name", givenNameClaimValue);
            //var familyNameClaimValue = principal.Claims.Where(claim => claim.Type == ClaimTypes.Surname).Select(claim => claim.Value).Single();
            //serviceRequestModel.Issuance.Claims.Add("family_name", familyNameClaimValue);
            //serviceRequestModel.Issuance.Claims.Add("birthdate", "1980-01-11");
            //serviceRequestModel.Issuance.Claims.Add("birthplace", "Abom State, Kingdom of Zamunda");
            //serviceRequestModel.Issuance.Claims.Add("document_number", "83774554");
            //serviceRequestModel.Issuance.Claims.Add("date_of_issuance", "2021-04-20");
            //serviceRequestModel.Issuance.Claims.Add("date_of_expiry", "2031-04-19");

            if (issuanceClaims != null)
            {
                serviceRequestModel.Issuance.Claims = issuanceClaims;

                if (!IsMobileRequest())
                {
                    const int pinLength = 4;
                    var pinMaxValue = (int)Math.Pow(10, pinLength) - 1;
                    var randomNumber = RandomNumberGenerator.GetInt32(1, pinMaxValue);

                    serviceRequestModel.Issuance.Pin = new PinModel
                    {
                        Value = string.Format("{0:D" + pinLength + "}", randomNumber),
                        Type = "numeric",
                        Length = pinLength
                    };
                }
            }

            //if (!IsMobileRequest())
            //{
            //    const int pinLength = 4;
            //    var pinMaxValue = (int) Math.Pow(10, pinLength) - 1;
            //    var randomNumber = RandomNumberGenerator.GetInt32(1, pinMaxValue);

            //    serviceRequestModel.Issuance.Pin = new PinModel
            //    {
            //        Value = string.Format("{0:D" + pinLength + "}", randomNumber),
            //        Type = "numeric",
            //        Length = pinLength
            //    };
            //}

            var serviceResponseModel = await _requestService.IssuanceRequest(serviceRequestModel);

            var cacheData = new
            {
                code = string.Empty,
                message = "Waiting for scan..."
            };

            _cache.Set(serviceRequestModel.Callback.State, JsonConvert.SerializeObject(cacheData));

            var apiResponseModel = new IssuanceRequestApiResponseModel
            {
                RequestId = serviceResponseModel.RequestId,
                Url = serviceResponseModel.Url,
                Expiry = serviceResponseModel.Expiry,
                QrCode = serviceResponseModel.QrCode,
                PinValue = serviceRequestModel.Issuance.Pin?.Value,
                State = serviceRequestModel.Callback.State
            };

            return Content(JsonConvert.SerializeObject(apiResponseModel), "application/json");
        }

        //[HttpGet("/api/issuer/issuance-response")]
        protected ActionResult IssuanceResponseBase([FromQuery] string state)
        {
            if (_cache.TryGetValue(state, out string serializedCacheData))
            {
                return Content(serializedCacheData, "application/json");
            }

            return Ok();
        }

        private string GetRequestBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}";
        }

        private bool IsMobileRequest()
        {
            var userAgents = Request.Headers["User-Agent"];
            return userAgents.Any(userAgent => userAgent.Contains("Android") || userAgent.Contains("iPhone"));
        }
    }
}
