using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public interface IRoatpRegisterApiClient
    {
        [Get("/api/v1/ukrlp/lookup/{ukprn}")]
        Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails([AliasAs("ukprn")] string ukprn);

        [Get("/api/v1/Organisation/register-status?ukprn={ukprn}")]
        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus([AliasAs("ukprn")] string ukprn);

        [Get("/api/v1/lookupData/providerTypes")]
        Task<IEnumerable<ProviderType>> GetProviderTypes();

        [Get("/api/v1/lookupData/organisationTypes?providerTypeId={providerTypeId}")]
        Task<IEnumerable<OrganisationType>> GetOrganisationTypes([AliasAs("providerTypeId")] int? providerTypeId);

        [Get("/api/v1/lookupData/organisationStatuses?providerTypeId={providerTypeId}")]
        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses([AliasAs("providerTypeId")] int? providerTypeId);

        [Get("/api/v1/lookupData/removedReasons")]
        Task<IEnumerable<RemovedReason>> GetRemovedReasons();
    }
}
