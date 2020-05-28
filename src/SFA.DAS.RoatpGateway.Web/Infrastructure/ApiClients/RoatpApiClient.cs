﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpApiClient : RoatpApiClientBase<RoatpApiClient>, IRoatpApiClient
    {
        public RoatpApiClient(HttpClient client, ILogger<RoatpApiClient> logger, IRoatpTokenService tokenService)
            : base(client, logger, tokenService)
        {
        }

        public async Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails(string ukprn)
        {
            return await Get<List<ProviderDetails>>($"/api/v1/ukrlp/lookup/{ukprn}");
        }

        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn)
        {
            return await Get<OrganisationRegisterStatus>($"/api/v1/Organisation/register-status?ukprn={ukprn}");
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId)
        {
            return await Get<List<OrganisationType>>($"/api/v1/lookupData/organisationTypes?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            return await Get<List<ProviderType>>($"/api/v1/lookupData/providerTypes");
        }

        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            return await Get<List<RemovedReason>>($"/api/v1/lookupData/removedReasons");
        }

        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId)
        {
            return await Get<List<OrganisationStatus>>($"/api/v1/lookupData/organisationStatuses?providerTypeId={providerTypeId}");
        }
    }
}
