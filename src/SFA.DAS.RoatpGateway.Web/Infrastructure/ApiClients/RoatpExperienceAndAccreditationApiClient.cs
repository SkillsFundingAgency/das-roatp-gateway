using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients.TokenService;

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
            return await Get<string>($"/Accreditation/{applicationId}/OfficeForStudents");
        }

        public async Task<InitialTeacherTraining> GetInitialTeacherTraining(Guid applicationId)
        {
            try
            {
                return await Get<InitialTeacherTraining>($"/Accreditation/{applicationId}/InitialTeacherTraining");
            }
            catch (Exception ex)
            {
                var message =
                    $"An error occurred when retrieving initial teacher training details from qna via apply for application {applicationId}";
                _logger.LogError(message, ex);
                throw new ExternalApiException(message, ex);
            }
        }

        public async Task<OfstedDetails> GetOfstedDetails(Guid applicationId)
        {
            return await Get<OfstedDetails>($"/Accreditation/{applicationId}/OfstedDetails");
        }
    }
}
