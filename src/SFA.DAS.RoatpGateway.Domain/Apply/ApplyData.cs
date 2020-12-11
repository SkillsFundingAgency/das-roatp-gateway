using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class ApplyData
    {
        public List<RoatpApplySequence> Sequences { get; set; }
        public ApplyDetails ApplyDetails { get; set; }
        public ApplyGatewayDetails GatewayReviewDetails { get; set; }
    }

}
