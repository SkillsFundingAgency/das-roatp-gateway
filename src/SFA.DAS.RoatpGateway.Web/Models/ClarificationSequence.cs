using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Models
{
    public class ClarificationSequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public List<ClarificationSection> Sections { get; set; }
    }

    public class ClarificationSection
    {
       
         public string PageTitle { get; set; }
         public string Comment { get; set; }
    }
}
