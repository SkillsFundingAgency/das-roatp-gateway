using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Domain.CompaniesHouse;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using System.Net.Http;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.Exceptions;
using System.Net.Http.Headers;
using SFA.DAS.AdminService.Common.Infrastructure;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient client, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService)
            : base(client, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(client.BaseAddress));
        }

        public async Task<RoatpApplicationResponse> GetApplication(Guid applicationId)
        {
            return await Get<RoatpApplicationResponse>($"/Application/{applicationId}");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetNewGatewayApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/NewApplications");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetInProgressGatewayApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/InProgressApplications");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetClosedGatewayApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/ClosedApplications");
        }

        public async Task StartGatewayReview(Guid applicationId, string reviewer)
        {
            await Post($"/GatewayReview/{applicationId}/StartReview", new { reviewer });
        }

        public async Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy)
        {
            await Post($"/GatewayReview/{applicationId}/Evaluate", new { isGatewayApproved, evaluatedBy });
        }

        public async Task StartAssessorReview(Guid applicationId, string reviewer)
        {
            await Post($"/Application/{applicationId}/StartAssessorReview", new { reviewer });
        }

        public async Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId)
        {
            return await Get<List<GatewayPageAnswerSummary>>($"/Gateway/Pages?applicationId={applicationId}");
        }

        public async Task<GatewayCommonDetails> GetPageCommonDetails(Guid applicationId, string pageId, string userName)
        {
            try
            {
                return await Get<GatewayCommonDetails>($"Gateway/Page/CommonDetails/{applicationId}/{pageId}/{userName}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Gateway common details", ex);
                throw new ExternalApiException("An error occurred when retrieving Gateway common details", ex);
            }
        }

        public async Task<ContactAddress> GetOrganisationAddress(Guid applicationId)
        {
            return await Get<ContactAddress>($"/Gateway/{applicationId}/OrganisationAddress");
        }

        public async Task<IcoNumber> GetIcoNumber(Guid applicationId)
        {
            return await Get<IcoNumber>($"/Gateway/{applicationId}/IcoNumber");
        }

        public async Task TriggerGatewayDataGathering(Guid applicationId, string userName)
        {
            try
            {
                await Get<object>($"Gateway/ApiChecks/{applicationId}/{userName}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Gateway Api checks details", ex);
                throw new ExternalApiException("An error occurred when retrieving Gateway Api checks details", ex);
            }
        }

        public async Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string userId, string username,
            string comments)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{pageId}' - Status '{status}' - UserName '{username}' - Comments '{comments}'");

            try
            {
                await Post($"/Gateway/Page/Submit", new { applicationId, pageId, status, comments, userId, username });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }
        }

        public async Task UpdateGatewayReviewStatusAndComment(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment, string userName)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-UpdateGatewayReviewStatusAndComment - ApplicationId '{applicationId}' - GatewayReviewStatus '{gatewayReviewStatus}' - GatewayReviewComment '{gatewayReviewComment}' - UserName '{userName}'");

            try
            {
                var responseCode = await Post($"/Gateway/UpdateGatewayReviewStatusAndComment", new { applicationId, gatewayReviewStatus, gatewayReviewComment, userName });
                if (responseCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Unable to update RoATP gateway review status, response code {responseCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient-UpdateGatewayReviewStatusAndComment - Error: '" + ex.Message + "'");
                throw;
            }

        }

        public async Task<ProviderDetails> GetUkrlpDetails(Guid applicationId)
        {
            try
            {
                return await Get<ProviderDetails>($"Gateway/UkrlpData/{applicationId}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving UKRLP details", ex);
                throw new ExternalApiException("An error occurred when retrieving UKRLP details", ex);
            }
        }

        public async Task<CompaniesHouseSummary> GetCompaniesHouseDetails(Guid applicationId)
        {
            try
            {
                return await Get<CompaniesHouseSummary>($"Gateway/CompaniesHouseData/{applicationId}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Companies House details", ex);
                throw new ExternalApiException("An error occurred when retrieving Companies House details", ex);
            }
        }

        public async Task<CharityCommissionSummary> GetCharityCommissionDetails(Guid applicationId)
        {
            try
            {
                return await Get<CharityCommissionSummary>($"Gateway/CharityCommissionData/{applicationId}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Charity Commission details", ex);
                throw new ExternalApiException("An error occurred when retrieving Charity Commission details", ex);
            }
        }

        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(Guid applicationId)
        {
            try
            {
                return await Get<OrganisationRegisterStatus>($"Gateway/RoatpRegisterData/{applicationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred when retrieving RoATP details", ex);
                throw new ExternalApiException("An error occurred when retrieving RoATP details", ex);
            }
        }

        public async Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId)
        {
            return await Get<DateTime?>($"Gateway/SourcesCheckedOn/{applicationId}");
        }

        public async Task<string> GetTradingName(Guid applicationId)
        {
            return await Get<string>($"/Gateway/{applicationId}/TradingName");
        }

        public async Task<string> GetProviderRouteName(Guid applicationId)
        {
            return await Get<string>($"/Gateway/{applicationId}/ProviderRouteName");
        }

        public async Task<string> GetWebsiteAddressSourcedFromUkrlp(Guid applicationId)
        {
            return await Get<string>($"/Gateway/{applicationId}/WebsiteAddressFromUkrlp");
        }


        public async Task<string> GetWebsiteAddressManuallyEntered(Guid applicationId)
        {
            return await Get<string>($"/Gateway/{applicationId}/WebsiteAddressManuallyEntered");
        }

        public async Task<string> GetOrganisationWebsiteAddress(Guid applicationId)
        {
            return await Get<string>($"/Gateway/{applicationId}/OrganisationWebsiteAddress");
        }


        public async Task<ContactDetails> GetContactDetails(Guid applicationId)
        {
            return await Get<ContactDetails>($"/Application/{applicationId}/Contact");
        }

    }
}
