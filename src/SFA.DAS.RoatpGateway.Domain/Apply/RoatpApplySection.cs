using System;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class RoatpApplySection
    {
        public Guid SectionId { get; set; }
        public int SectionNo { get; set; }
        public string Status { get; set; }
        public bool NotRequired { get; set; }
    }
}
