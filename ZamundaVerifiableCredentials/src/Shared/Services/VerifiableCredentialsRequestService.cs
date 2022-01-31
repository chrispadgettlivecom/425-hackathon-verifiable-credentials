using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace Shared.Services
{
    public class VerifiableCredentialsRequestService : IVerifiableCredentialsRequestService
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IConfidentialClientApplication _confidentialClientApplication;
        private readonly JsonSerializerSettings _serializerSettings;

        public VerifiableCredentialsRequestService(IOptions<AppSettingsModel> appSettingsAccessor, IConfidentialClientApplication confidentialClientApplication)
        {
            if (appSettingsAccessor == null)
            {
                throw new ArgumentNullException(nameof(appSettingsAccessor));
            }

            _appSettings = appSettingsAccessor.Value;
            _confidentialClientApplication = confidentialClientApplication ?? throw new ArgumentNullException(nameof(confidentialClientApplication));

            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task<IssuanceRequestServiceResponseModel> IssuanceRequest(IssuanceRequestServiceRequestModel requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }

            try
            {
                var accessTokenResult = await GetAccessToken();

                if (string.IsNullOrEmpty(accessTokenResult.access_token))
                {
                    //return BadRequest(new
                    //{
                    //    error = accessTokenResult.error,
                    //    error_description = accessTokenResult.error_description
                    //});
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.access_token);
                    var serializedRequestModel = JsonConvert.SerializeObject(requestModel, _serializerSettings);

                    using (var requestContent = new StringContent(serializedRequestModel, Encoding.UTF8, "application/json"))
                    {
                        using (var response = await client.PostAsync(_appSettings.RequestServiceEndpointAddress, requestContent))
                        {
                            response.EnsureSuccessStatusCode();
                            var serializedResponseModel = await response.Content.ReadAsStringAsync();
                            var responseModel = JsonConvert.DeserializeObject<IssuanceRequestServiceResponseModel>(serializedResponseModel);
                            return responseModel;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<(string access_token, string error, string error_description)> GetAccessToken()
        {
            try
            {
                var scopes = new[]
                {
                    _appSettings.RequestServiceScope
                };

                var authenticationResult = await _confidentialClientApplication.AcquireTokenForClient(scopes)
                    .ExecuteAsync();

                return (authenticationResult.AccessToken, string.Empty, string.Empty);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                return (string.Empty, "500", "Invalid scope");
            }
            catch (MsalServiceException)
            {
                return (string.Empty, "500", "Failed getting an access token");
            }
        }
    }
}
