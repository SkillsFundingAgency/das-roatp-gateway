using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;

public class RoatpExperienceAndAccreditationApiClient : ApiClientBase<RoatpExperienceAndAccreditationApiClient>, IRoatpExperienceAndAccreditationApiClient
{
    public RoatpExperienceAndAccreditationApiClient(HttpClient client, ILogger<RoatpExperienceAndAccreditationApiClient> logger) : base(client, logger)
    { }

    public async Task<SubcontractorDeclaration> GetSubcontractorDeclaration(Guid applicationId)
    {
        return await Get<SubcontractorDeclaration>($"/Accreditation/{applicationId}/SubcontractDeclaration");
    }

    public async Task<FileStreamResult> GetSubcontractorDeclarationContractFile(Guid applicationId)
    {
        var response = await GetResponse($"/Accreditation/{applicationId}/SubcontractDeclaration/ContractFile");

        var fileStream = await response.Content.ReadAsStreamAsync();
        var result = new FileStreamResult(fileStream, response.Content.Headers.ContentType.MediaType);
        result.FileDownloadName = response.Content.Headers.ContentDisposition.FileName.Replace("\"", "");
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
        if (response.IsSuccessStatusCode) return await response.Content.ReadAsAsync<string>();
        var message =
            $"An error occurred when retrieving office for students from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
        _logger.LogError(message);
        throw new ExternalApiException(message);
    }

    public async Task<InitialTeacherTraining> GetInitialTeacherTraining(Guid applicationId)
    {
        var response = await GetResponse($"/Accreditation/{applicationId}/InitialTeacherTraining");
        if (response.IsSuccessStatusCode) return await response.Content.ReadAsAsync<InitialTeacherTraining>();
        var message =
            $"An error occurred when retrieving initial teacher training details from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
        _logger.LogError(message);
        throw new ExternalApiException(message);
    }

    public async Task<OfstedDetails> GetOfstedDetails(Guid applicationId)
    {
        var response = await GetResponse($"/Accreditation/{applicationId}/OfstedDetails");
        if (response.IsSuccessStatusCode) return await response.Content.ReadAsAsync<OfstedDetails>();
        var message =
            $"An error occurred when retrieving ofsted details from qna via apply for application {applicationId} with error message {response.ReasonPhrase}";
        _logger.LogError(message);
        throw new ExternalApiException(message);
    }
}
