using System;

namespace SFA.DAS.RoatpGateway.Domain.CompaniesHouse
{
    public class DirectorInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public DateTime? AppointedDate { get; set; }
        public DateTime? ResignedDate { get; set; }

        public bool Active
        {
            get { return AppointedDate.HasValue && !ResignedDate.HasValue; }
        }
    }
}
