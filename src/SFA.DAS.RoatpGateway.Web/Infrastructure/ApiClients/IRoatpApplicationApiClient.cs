using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Domain.Apply;
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
        Task<Apply> GetApplication(Guid applicationId);
        Task<List<RoatpApplicationSummaryItem>> GetNewGatewayApplications();
        Task<List<RoatpApplicationSummaryItem>> GetInProgressGatewayApplications();
        Task<List<RoatpApplicationSummaryItem>> GetClosedGatewayApplications();
        Task<GetGatewayApplicationCountsResponse> GetApplicationCounts();

        Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy);

        Task WithdrawApplication(Guid applicationId, string comments, string userId, string userName);

        Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId);
        Task<GatewayCommonDetails> GetPageCommonDetails(Guid applicationId, string pageId, string userName);
        Task<ContactAddress> GetOrganisationAddress(Guid applicationId);
        Task<IcoNumber> GetIcoNumber(Guid applicationId);

        Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string userId, string username, string comments);
        Task UpdateGatewayReviewStatusAndComment(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment, string userId, string userName);
        Task UpdateGatewayReviewStatusAsClarification(Guid applicationId, string userId, string userName);

        Task<ProviderDetails> GetUkrlpDetails(Guid applicationId);

        Task<CompaniesHouseSummary> GetCompaniesHouseDetails(Guid applicationId);

        Task<CharityCommissionSummary> GetCharityCommissionDetails(Guid applicationId);

        Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId);

        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(Guid applicationId);

        Task<string> GetTwoInTwelveMonths(Guid applicationId);
        Task<string> GetTradingName(Guid applicationId);
        Task<string> GetProviderRouteName(Guid applicationId);
        Task<string> GetWebsiteAddressSourcedFromUkrlp(Guid applicationId);
        Task<string> GetWebsiteAddressManuallyEntered(Guid applicationId);
        Task<string> GetOrganisationWebsiteAddress(Guid applicationId);

        Task<ContactDetails> GetContactDetails(Guid applicationId);

    }
}
