using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Domain.CompaniesHouse;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.Exceptions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient client, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService)
            : base(client, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(client.BaseAddress).Result);
        }

        public async Task<Apply> GetApplication(Guid applicationId)
        {
            return await Get<Apply>($"/Application/{applicationId}");
        }

        public async Task<GetGatewayApplicationCountsResponse> GetApplicationCounts(string searchTerm)
        {
            return await Get<GetGatewayApplicationCountsResponse>($"/GatewayReview/Counts?searchTerm={searchTerm}");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetNewGatewayApplications(string searchTerm, string sortOrder)
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/NewApplications?searchTerm={searchTerm}&sortOrder={sortOrder}");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetInProgressGatewayApplications(string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/InProgressApplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetClosedGatewayApplications(string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/ClosedApplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
        }

        public async Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string userId, string userName)
        {
            await Post($"/GatewayReview/{applicationId}/Evaluate", new { isGatewayApproved, userId, userName });
        }

        public async Task WithdrawApplication(Guid applicationId, string comments, string userId, string userName)
        {
            await Post($"/GatewayReview/{applicationId}/Withdraw", new { comments, userId, userName });
        }

        public async Task RemoveApplication(Guid applicationId, string comments, string externalComments, string userId, string userName)
        {
            await Post($"/GatewayReview/{applicationId}/Remove", new { comments, externalComments, userId, userName });
        }

        public async Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId)
        {
            return await Get<List<GatewayPageAnswerSummary>>($"/Gateway/{applicationId}/Pages");
        }

        public async Task<GatewayCommonDetails> GetPageCommonDetails(Guid applicationId, string pageId, string userId, string userName)
        {
            try
            {
                return await Post<GatewayCommonDetailsRequest, GatewayCommonDetails>($"Gateway/{applicationId}/CommonDetails", new GatewayCommonDetailsRequest(pageId, userId, userName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when retrieving Gateway common details");
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

        public async Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string userId, string username,
            string comments)
        {
            await SubmitGatewayPageAnswer(applicationId, pageId, status, userId, username, comments, null);
        }


        public async Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string userId, string username,
            string comments, string clarificationAnswer)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitGatewayPageAnswer - ApplicationId '{0}' - PageId '{1}' - Status '{2}' - UserName '{3}' - Comments '{4}' - ClarificationAnswer '{5}'",
            applicationId, pageId, status, username, comments, clarificationAnswer);

            try
            {
                await Post($"/Gateway/Page/Submit", new { applicationId, pageId, status, comments, userId, username, clarificationAnswer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }
        }

        public async Task SubmitGatewayPageAnswerClarification(Guid applicationId, string pageId, string status, string userId, string username,
            string comments, string clarificationAnswer)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitGatewayPageAnswerClarification - ApplicationId '{0}' - PageId '{1}' - Status '{2}' - UserName '{3}' - Comments '{4}' - ClarificationAnswer '{5}'",
                applicationId, pageId, status, username, comments, clarificationAnswer);

            try
            {
                await Post($"/Gateway/Page/SubmitClarification", new { applicationId, pageId, status, comments, userId, username, clarificationAnswer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient - SubmitGatewayPageAnswerClarification - Error: '" + ex.Message + "'");
            }
        }

        public async Task SubmitGatewayPageAnswerPostClarification(Guid applicationId, string pageId, string status, string userId, string username,
            string comments, string clarificationAnswer)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitGatewayPageAnswerPostClarification - ApplicationId '{0}' - PageId '{1}' - Status '{2}' - UserName '{3}' - Comments '{4}' - ClarificationAnswer '{5}'",
                applicationId, pageId, status, username, comments);

            try
            {
                await Post($"/Gateway/Page/SubmitPostClarification", new { applicationId, pageId, status, comments, userId, username, clarificationAnswer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient - SubmitGatewayPageAnswerPostClarification - Error: '" + ex.Message + "'");
            }
        }

        public async Task UpdateGatewayReviewStatusAndComment(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment, string gatewayReviewExternalComment, int? subcontractingLimit, string userId, string userName)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-UpdateGatewayReviewStatusAndComment - ApplicationId '{0}' - GatewayReviewStatus '{1}' - GatewayReviewComment '{2}'- GatewayReviewExternalComment '{3}' - UserName '{4}'",
                applicationId, gatewayReviewStatus, gatewayReviewComment, gatewayReviewExternalComment, userName);


            try
            {
                var responseCode = await Post($"/Gateway/UpdateGatewayReviewStatusAndComment", new { applicationId, gatewayReviewStatus, gatewayReviewComment, gatewayReviewExternalComment, subcontractingLimit, userId, userName });
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

        public async Task UpdateGatewayReviewStatusAsClarification(Guid applicationId, string userId, string userName)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-UpdateGatewayReviewStatusAsClarification - ApplicationId '{0}' - UserName '{1}'",
                applicationId, userName);

            try
            {
                var responseCode = await Post($"/Gateway/UpdateGatewayClarification", new { applicationId, userId, userName });
                if (responseCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Unable to update RoATP gateway review status as clarification, response code {responseCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient-UpdateGatewayReviewStatusAsClarification - Error: '{0}'", ex.Message);
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
                _logger.LogError(ex, "An error occurred when retrieving UKRLP details");
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
                _logger.LogError(ex, "An error occurred when retrieving Companies House details");
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
                _logger.LogError(ex, "An error occurred when retrieving Charity Commission details");
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
                _logger.LogError(ex, "An error occurred when retrieving RoATP details");
                throw new ExternalApiException("An error occurred when retrieving RoATP details", ex);
            }
        }

        public async Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId)
        {
            return await Get<DateTime?>($"Gateway/SourcesCheckedOn/{applicationId}");
        }

        public async Task<string> GetTradingName(Guid applicationId)
        {
            var response = await GetResponse($"/Gateway/{applicationId}/TradingName");
            if (response.IsSuccessStatusCode) return await response.Content.ReadAsAsync<string>();
            var message =
                $"An error occurred when retrieving trading name from qna via apply for application {applicationId} with error message '{response.ReasonPhrase}'";
            _logger.LogError(message);
            throw new ExternalApiException(message);
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
            var response = await GetResponse($"/Gateway/{applicationId}/OrganisationWebsiteAddress");
            if (response.IsSuccessStatusCode) return await response.Content.ReadAsAsync<string>();

            var message =
                    $"An error occurred when retrieving website address from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
            _logger.LogError(message);
            throw new ExternalApiException(message);
        }


        public async Task<ContactDetails> GetContactDetails(Guid applicationId)
        {
            return await Get<ContactDetails>($"/Application/{applicationId}/Contact");
        }

        public async Task<bool> UploadSubcontractorDeclarationClarificationFile(Guid applicationId, string userId, string userName,
            IFormFileCollection clarificationFiles)
        {
            var fileName = string.Empty;
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(userId), "userId");
            content.Add(new StringContent(userName), "userName");

            if (clarificationFiles != null && clarificationFiles.Any())
            {
                foreach (var file in clarificationFiles)
                {
                    fileName = file.FileName;
                    var fileContent = new StreamContent(file.OpenReadStream())
                    {
                        Headers =
                        {
                            ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType)
                        }
                    };
                    content.Add(fileContent, file.FileName, file.FileName);
                }

                try
                {
                    var response = await _httpClient.PostAsync($"/Gateway/SubcontractorDeclarationClarification/{applicationId}/Upload", content);

                    return response.StatusCode == HttpStatusCode.OK;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex,
                        $"Error when submitting Subcontractor Declaration Clarification File update for Application: {applicationId} | Filename: {fileName}");
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> RemoveSubcontractorDeclarationClarificationFile(Guid applicationId, string userId, string userName, string fileName)
        {
            try
            {
                var response = await Post($"/Gateway/SubcontractorDeclarationClarification/{applicationId}/Remove", new { userId, userName, fileName });

                return response == HttpStatusCode.OK;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    $"Error when removing Subcontractor Declaration Clarification File for Application: {applicationId} | Filename: {fileName}");
                return false;
            }
        }

        public async Task<ApplicationOversightDetails> GetOversightDetails(Guid applicationId)
        {
            return await Get<ApplicationOversightDetails>($"/Oversights/{applicationId}");
        }
    }
}
