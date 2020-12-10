using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Web.Models
{
    public class ClarificationSequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public List<ClarificationSection> Sections { get; set; }
    }
}
