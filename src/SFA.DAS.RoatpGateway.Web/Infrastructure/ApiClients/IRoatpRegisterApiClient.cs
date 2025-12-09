using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SFA.DAS.RoatpGateway.Domain.Roatp;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;

public interface IRoatpRegisterApiClient
{
    [Get("/organisations/{ukprn}")]
    Task<ApiResponse<OrganisationResponse>> GetOrganisationRegisterStatus(string ukprn);
    [Get("/organisation-types")]
    Task<IEnumerable<OrganisationType>> GetOrganisationTypes([Query] int? providerTypeId);
    [Get("/removed-reasons")]
    Task<ApiResponse<RemovedReasonResponse>> GetRemovedReasons();
}
