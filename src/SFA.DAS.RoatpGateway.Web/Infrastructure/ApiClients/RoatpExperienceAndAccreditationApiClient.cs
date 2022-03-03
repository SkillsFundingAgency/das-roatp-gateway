using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public class RoatpExperienceAndAccreditationApiClient : ApiClientBase<RoatpExperienceAndAccreditationApiClient>, IRoatpExperienceAndAccreditationApiClient
    {
        public RoatpExperienceAndAccreditationApiClient(HttpClient client, ILogger<RoatpExperienceAndAccreditationApiClient> logger, IRoatpApplicationTokenService tokenService)
            : base(client, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(client.BaseAddress));
        }

        public async Task<SubcontractorDeclaration> GetSubcontractorDeclaration(Guid applicationId)
        {
            return await Get<SubcontractorDeclaration>($"/Accreditation/{applicationId}/SubcontractDeclaration");
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFile(Guid applicationId)
        {
            var response = await GetResponse($"/Accreditation/{applicationId}/SubcontractDeclaration/ContractFile");

            var fileStream = await response.Content.ReadAsStreamAsync();
            var result = new FileStreamResult(fileStream, response.Content.Headers.ContentType.MediaType);
            result.FileDownloadName = response.Content.Headers.ContentDisposition.FileName.Replace("\"","");
            return result;
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFileClarification(Guid applicationId, string fileName)
        {
            var response = await GetResponse($"/Accreditation/{applicationId}/SubcontractDeclaration/ContractFileClarification/{fileName}");

            var fileStream = await response.Content.ReadAsStreamAsync();
            var result = new FileStreamResult(fileStream, response.Content.Headers.ContentType.MediaType);
            result.FileDownloadName = response.Content.Headers.ContentDisposition.FileName.Replace("\"", "");
            return result;
        }

        public async Task<string> GetOfficeForStudents(Guid applicationId)
        { 
            var response = await GetResponse($"/Accreditation/{applicationId}/OfficeForStudents");
            if (response.StatusCode == HttpStatusCode.OK) return await response.Content.ReadAsAsync<string>();
            var message =
                $"An error occurred when retrieving office for students from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
            _logger.LogError(message);
            throw new ExternalApiException(message);
        }

        public async Task<InitialTeacherTraining> GetInitialTeacherTraining(Guid applicationId)
        {
                var response = await GetResponse($"/Accreditation/{applicationId}/InitialTeacherTraining");
                if (response.StatusCode == HttpStatusCode.OK) return await response.Content.ReadAsAsync<InitialTeacherTraining>();
                var message =
                    $"An error occurred when retrieving initial teacher training details from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
                _logger.LogError(message);
                throw new ExternalApiException(message);
        }

        public async Task<OfstedDetails> GetOfstedDetails(Guid applicationId)
        {
            var response = await GetResponse($"/Accreditation/{applicationId}/OfstedDetails");
            if (response.StatusCode == HttpStatusCode.OK) return await response.Content.ReadAsAsync<OfstedDetails>();
            var message =
                $"An error occurred when retrieving ofsted details from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
            _logger.LogError(message);
            throw new ExternalApiException(message);
        }
    }
}
