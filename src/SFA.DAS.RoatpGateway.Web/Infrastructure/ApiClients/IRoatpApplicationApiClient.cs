using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;
using SFA.DAS.RoatpGateway.Domain.CompaniesHouse;
using SFA.DAS.RoatpGateway.Domain.Roatp;
using SFA.DAS.RoatpGateway.Domain.Ukrlp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<RoatpApplicationResponse> GetApplication(Guid Id);
        Task<List<RoatpApplicationSummaryItem>> GetNewGatewayApplications();
        Task<List<RoatpApplicationSummaryItem>> GetInProgressGatewayApplications();
        Task<List<RoatpApplicationSummaryItem>> GetClosedGatewayApplications();
        Task StartGatewayReview(Guid applicationId, string reviewer);
        Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy);

        Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId);
        Task<GatewayCommonDetails> GetPageCommonDetails(Guid applicationId, string pageId, string userName);
        Task<ContactAddress> GetOrganisationAddress(Guid applicationId);
        Task<IcoNumber> GetIcoNumber(Guid applicationId);
        Task TriggerGatewayDataGathering(Guid applicationId, string userName);

        Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string userId, string username, string comments);
        Task UpdateGatewayReviewStatusAndComment(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment, string userName);
        Task<ProviderDetails> GetUkrlpDetails(Guid applicationId);

        Task<CompaniesHouseSummary> GetCompaniesHouseDetails(Guid applicationId);

        Task<CharityCommissionSummary> GetCharityCommissionDetails(Guid applicationId);

        Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId);

        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(Guid applicationId);

        Task<string> GetTradingName(Guid applicationId);
        Task<string> GetProviderRouteName(Guid applicationId);
        Task<string> GetWebsiteAddressSourcedFromUkrlp(Guid applicationId);
        Task<string> GetWebsiteAddressManuallyEntered(Guid applicationId);
        Task<string> GetOrganisationWebsiteAddress(Guid applicationId);
    }
}
