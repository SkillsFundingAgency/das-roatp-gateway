using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Domain.Apply
{
    public class RoatpApplyData
    {
        public List<RoatpApplySequence> Sequences { get; set; }
        public RoatpApplyDetails ApplyDetails { get; set; }
    }

}
