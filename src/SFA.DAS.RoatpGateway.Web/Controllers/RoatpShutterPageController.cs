using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    [Authorize]
    public class RoatpShutterPageController : Controller
    {
        [Route("ExternalApisUnavailable")]
        public IActionResult ExternalApisUnavailable()
        {
            return View("~/Views/Roatp/Apply/ExternalApisUnavailable.cshtml");
        }
    }
}
