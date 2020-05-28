using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IGatewayOrganisationChecksOrchestrator
    {
        Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest getLegalNameRequest);
        Task<TradingNamePageViewModel> GetTradingNameViewModel(GetTradingNameRequest getTradingNameRequest);
        Task<OrganisationStatusViewModel> GetOrganisationStatusViewModel(GetOrganisationStatusRequest getOrganisationStatusRequest);
        Task<AddressCheckViewModel> GetAddressViewModel(GetAddressRequest request);
        Task<IcoNumberViewModel> GetIcoNumberViewModel(GetIcoNumberRequest request);
        Task<WebsiteViewModel> GetWebsiteViewModel(GetWebsiteRequest request);
        Task<OrganisationRiskViewModel> GetOrganisationRiskViewModel(GetOrganisationRiskRequest request);
    }
}
