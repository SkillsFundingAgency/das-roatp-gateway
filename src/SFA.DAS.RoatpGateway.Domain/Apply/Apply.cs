using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class Apply
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
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }

}
