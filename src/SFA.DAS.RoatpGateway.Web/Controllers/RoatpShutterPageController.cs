using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    [Authorize]
    public class RoatpShutterPageController : Controller
    {
        [Route("ServiceUnavailable")]
        public IActionResult ExternalApisUnavailable()
        {
            return View("~/Views/ErrorPage/ServiceUnavailable.cshtml");
        }
    }
}
