
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Domain.Roatp;

public class RemovedReasonResponse
{
    public IEnumerable<RemovedReason> ReasonsForRemoval { get; set; }
}