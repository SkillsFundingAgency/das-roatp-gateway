using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpGateway.Web.Models;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpOrganisationSummaryApiClient : ApiClientBase<RoatpOrganisationSummaryApiClient>, IRoatpOrganisationSummaryApiClient
    {
        private const string RoutePath = "organisation";

        public RoatpOrganisationSummaryApiClient(HttpClient client, ILogger<RoatpOrganisationSummaryApiClient> logger, IRoatpApplicationTokenService tokenService)
            : base(client, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(client.BaseAddress));
        }

        public async Task<string> GetTypeOfOrganisation(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving type of organisation from applicationId [{applicationId}]");
            return await Get<string>($"{RoutePath}/TypeOfOrganisation/{applicationId}");
        }

        public async Task<string> GetCompanyNumber(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving company number from applicationId [{applicationId}]");
            return await Get<string>($"{RoutePath}/CompanyNumber/{applicationId}");
        }

        public async Task<string> GetCharityNumber(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving charity number from applicationId [{applicationId}]");
            return await Get<string>($"{RoutePath}/CharityNumber/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetDirectorsFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of directors submitted in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/DirectorData/Submitted/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetDirectorsFromCompaniesHouse(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of directors from companies house in ApplyData from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/DirectorData/CompaniesHouse/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetPscsFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of pscs in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/PscData/Submitted/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetPscsFromCompaniesHouse(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of pscs from companies house in ApplyData from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/PscData/CompaniesHouse/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetTrusteesFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of trustees in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/TrusteeData/Submitted/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetTrusteesFromCharityCommission(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of trustees from charity commission in ApplyData from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/TrusteeData/CharityCommission/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetWhosInControlFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of Whos in control in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/WhosInControlData/Submitted/{applicationId}");
        }
    }
}
