using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain.Roatp;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;

public interface IRoatpRegisterApiClient
{
    Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn);
    Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId);
    Task<IEnumerable<RemovedReason>> GetRemovedReasons();
}
