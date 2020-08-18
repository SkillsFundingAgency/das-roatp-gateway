using SFA.DAS.RoatpGateway.Domain.Apply;
using System;

namespace SFA.DAS.RoatpGateway.Domain
{
    public class RoatpApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string ApplicationStatus { get; set; }
        public string AssessorReviewStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public RoatpApplyData ApplyData { get; set; }
        public FinancialReviewDetails FinancialGrade { get; set; }
        public string CreatedBy { get; set; }
    }

}
