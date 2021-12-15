using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.Exceptions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class OuterApiClient : ApiClientBase<OuterApiClient>, IOuterApiClient
    {
        public OuterApiClient(HttpClient client, ILogger<OuterApiClient> logger)
            : base(client, logger)
        {
        }

        public async Task<CharityDetails> GetCharityDetails(string registrationNumber)
        {
            try
            {
                return await Get<CharityDetails>($"Charities/{registrationNumber}");
            }
            catch (OuterApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Charity Commission details from APIM Roatp", ex);
                throw new ExternalApiException("An error occurred when retrieving Charity Commission details", ex);
            }
        }
    }
}