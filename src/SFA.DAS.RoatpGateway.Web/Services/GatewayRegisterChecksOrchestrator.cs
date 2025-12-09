using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Refit;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Web.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services;

public class GatewayRegisterChecksOrchestrator : IGatewayRegisterChecksOrchestrator
{
    private readonly IRoatpApplicationApiClient _applyApiClient;
    private readonly IRoatpRegisterApiClient _roatpApiClient;
    private readonly ILogger<GatewayRegisterChecksOrchestrator> _logger;

    private readonly KeyValuePair<int, string>[] ProviderTypes = [
        new(1, "Main provider"),
        new(2, "Employer provider"),
        new(3, "Supporting provider")
    ];
    private readonly KeyValuePair<int, string>[] OrganisationStatuses = [
        new(0, "Removed"),
        new(1, "Active"),
        new(2, "Active - but not taking on apprentices"),
        new(3, "On-boarding")
    ];

    public GatewayRegisterChecksOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpRegisterApiClient roatpApiClient,
                                                 ILogger<GatewayRegisterChecksOrchestrator> logger)
    {
        _applyApiClient = applyApiClient;
        _roatpApiClient = roatpApiClient;
        _logger = logger;
    }

    public async Task<RoatpPageViewModel> GetRoatpViewModel(GetRoatpRequest request)
    {
        var pageId = GatewayPageIds.Roatp;
        _logger.LogInformation("Retrieving RoATP details for application {ApplicationId}", request.ApplicationId);

        var model = new RoatpPageViewModel();
        await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.RegisterChecks,
                                                pageId,
                                                request.UserId,
                                                request.UserName,
                                                RoatpGatewayConstants.Captions.RegisterChecks,
                                                RoatpGatewayConstants.Headings.Roatp,
                                                NoSelectionErrorMessages.Errors[GatewayPageIds.Roatp]);

        model.ApplyProviderRoute = await _applyApiClient.GetProviderRouteName(model.ApplicationId);

        ApiResponse<OrganisationResponse> organisationResponse = await _roatpApiClient.GetOrganisationRegisterStatus(model.Ukprn);

        if (organisationResponse.IsSuccessStatusCode)
        {
            OrganisationRegisterStatus registerStatus = organisationResponse.Content;
            model.RoatpUkprnOnRegister = registerStatus.UkprnOnRegister;
            model.RoatpStatusDate = registerStatus.StatusDate;
            model.RoatpProviderRoute = GetProviderRoute(registerStatus.ProviderTypeId);
            model.RoatpStatus = GetProviderStatus(registerStatus.StatusId);
            model.RoatpRemovedReason = await GetRemovedReason(registerStatus.RemovedReasonId);
        }

        return model;
    }

    public async Task<RoepaoPageViewModel> GetRoepaoViewModel(GetRoepaoRequest request)
    {
        var pageId = GatewayPageIds.Roepao;
        _logger.LogInformation("Retrieving RoEPAO details for application {ApplicationId}", request.ApplicationId);

        var model = new RoepaoPageViewModel();
        await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewaySequences.RegisterChecks,
                                                pageId,
                                                request.UserId,
                                                request.UserName,
                                                RoatpGatewayConstants.Captions.RegisterChecks,
                                                RoatpGatewayConstants.Headings.Roepao,
                                                NoSelectionErrorMessages.Errors[GatewayPageIds.Roepao]);

        return model;
    }

    private string GetProviderRoute(int? providerTypeId)
        => providerTypeId.HasValue ? ProviderTypes.FirstOrDefault(pt => pt.Key == providerTypeId.Value).Value : null;

    private string GetProviderStatus(int? providerStatusId)
        => providerStatusId.HasValue ? OrganisationStatuses.FirstOrDefault(s => s.Key == providerStatusId.Value).Value : null;

    private async Task<string> GetRemovedReason(int? providerRemovedReasonId)
    {
        string reason = null;

        if (providerRemovedReasonId.HasValue)
        {
            var roatpRemovedReasons = await _roatpApiClient.GetRemovedReasons();

            reason = roatpRemovedReasons.Content.ReasonsForRemoval.FirstOrDefault(rm => rm.Id == providerRemovedReasonId.Value)?.Description;
        }

        return reason;
    }
}
