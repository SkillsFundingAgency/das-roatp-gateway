using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class ErrorPageController : Controller
    {
        [Route("ErrorPage/404")]
        public IActionResult PageNotFound()
        {
            return View("~/Views/ErrorPage/PageNotFound.cshtml");
        }

        [Route("ErrorPage/500")]
        public IActionResult ServiceErrorHandler()
        {
            return RedirectToAction("ServiceError");
        }

        [Route("problem-with-service")]
        public IActionResult ServiceError()
        {
            return View("~/Views/ErrorPage/ServiceError.cshtml");
        }

        [Route("ErrorPage/503")]
        public IActionResult ServiceUnavailableHandler()
        {
            return RedirectToAction("ServiceUnavailable");
        }

        [Route("service-unavailable")]
        public IActionResult ServiceUnavailable()
        {
            return View("~/Views/ErrorPage/ServiceUnavailable.cshtml");
        }
    }
}