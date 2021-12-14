using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Domain.CharityCommission
{
    public class CharityDetails
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        public string CharityNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? RemovalDate { get; set; }

        public List<TrusteeInformation> Trustees { get; set; }
    }
}