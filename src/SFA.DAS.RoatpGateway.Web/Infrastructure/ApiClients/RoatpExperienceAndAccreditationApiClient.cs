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
            result.FileDownloadName = response.Content.Headers.ContentDisposition.FileName;
            return result;
        }

        public async Task<string> GetOfficeForStudents(Guid applicationId)
        {
            return await Get<string>($"/Accreditation/{applicationId}/OfficeForStudents");
        }

        public async Task<InitialTeacherTraining> GetInitialTeacherTraining(Guid applicationId)
        {
            return await Get<InitialTeacherTraining>($"/Accreditation/{applicationId}/InitialTeacherTraining");
        }

        public async Task<OfstedDetails> GetOfstedDetails(Guid applicationId)
        {
            return await Get<OfstedDetails>($"/Accreditation/{applicationId}/OfstedDetails");
        }
    }
}
