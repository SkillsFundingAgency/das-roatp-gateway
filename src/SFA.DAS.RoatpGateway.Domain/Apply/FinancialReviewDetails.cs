using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class FinancialReviewDetails
    {
        public string SelectedGrade { get; set; }
        public DateTime? FinancialDueDate { get; set; }
        public string GradedBy { get; set; }
        public DateTime? GradedDateTime { get; set; }
        public string Comments { get; set; }
        public List<FinancialEvidence> FinancialEvidences { get; set; }
    }

    public class FinancialEvidence
    {
        public string Filename { get; set; }
    }
}
