using SFA.DAS.RoatpGateway.Web.Models;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class PeopleInControlHighRiskPageViewModel : RoatpGatewayPageViewModel
    {
        public string TypeOfOrganisation { get; set; }

        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public PeopleInControlHighRiskData CompanyDirectorsData { get; set; }
        public PeopleInControlHighRiskData PscData { get; set; }
        public PeopleInControlHighRiskData TrusteeData { get; set; }
        public PeopleInControlHighRiskData WhosInControlData { get; set; }
    }
}
