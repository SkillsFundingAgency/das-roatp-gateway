using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;

public interface IRoatpRegisterApiClient
{
    Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails(string ukprn);
    Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn);
    Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId);
    Task<IEnumerable<RemovedReason>> GetRemovedReasons();
}
