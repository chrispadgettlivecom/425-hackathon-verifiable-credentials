using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IVerifiableCredentialsRequestService
    {
        Task<IssuanceRequestServiceResponseModel> IssuanceRequest(IssuanceRequestServiceRequestModel requestModel);
    }
}
