using System;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class Apply
    {
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }

        public string ApplicationStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public FinancialReviewDetails FinancialGrade { get; set; }

        public string GatewayUserId { get; set; }
        public string GatewayUserName { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public string Comments { get; set; }
        public string ExternalComments { get; set; }
    }

}
