using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public interface IRoatpApiClient
    {
        Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails(string ukprn);
        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn);
        Task<IEnumerable<ProviderType>> GetProviderTypes();
        Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId);
        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId);
        Task<IEnumerable<RemovedReason>> GetRemovedReasons();
    }
}
