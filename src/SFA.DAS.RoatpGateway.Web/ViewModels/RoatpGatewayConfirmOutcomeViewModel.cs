using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public class RoatpGatewayConfirmOutcomeViewModel: RoatpGatewayOutcomeViewModel
    {

        [Required(ErrorMessage = "Select if you are sure you want to pass this application")]
        public string ConfirmGatewayOutcome { get; set; }
    }


    public class RoatpGatewayFailOutcomeViewModel : RoatpGatewayOutcomeViewModel
    {

        [Required(ErrorMessage = "Select if you are sure you want to fail this application")]
        public string ConfirmGatewayOutcome { get; set; }
    }

    public class RoatpGatewayRejectedOutcomeViewModel : RoatpGatewayOutcomeViewModel
    {

        [Required(ErrorMessage = "Select if you are sure you want to reject this application")]
        public string ConfirmGatewayOutcome { get; set; }
    }
}
