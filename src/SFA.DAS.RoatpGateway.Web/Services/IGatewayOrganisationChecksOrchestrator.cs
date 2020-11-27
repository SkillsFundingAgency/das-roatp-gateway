using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IGatewayOrganisationChecksOrchestrator
    {
        Task<TwoInTwelveMonthsViewModel> GetTwoInTwelveMonthsViewModel(GetTwoInTwelveMonthsRequest request);
        Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest request);
        Task<TradingNamePageViewModel> GetTradingNameViewModel(GetTradingNameRequest request);
        Task<OrganisationStatusViewModel> GetOrganisationStatusViewModel(GetOrganisationStatusRequest request);
        Task<AddressCheckViewModel> GetAddressViewModel(GetAddressRequest request);
        Task<IcoNumberViewModel> GetIcoNumberViewModel(GetIcoNumberRequest request);
        Task<WebsiteViewModel> GetWebsiteViewModel(GetWebsiteRequest request);
        Task<OrganisationRiskViewModel> GetOrganisationRiskViewModel(GetOrganisationRiskRequest request);
    }
}
