using System;

namespace SFA.DAS.RoatpGateway.Domain.CompaniesHouse
{
    public class PersonWithSignificantControlInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime? NotifiedDate { get; set; }
        public DateTime? CeasedDate { get; set; }

        public bool Active
        {
            get { return !CeasedDate.HasValue; }
        }
    }
}
