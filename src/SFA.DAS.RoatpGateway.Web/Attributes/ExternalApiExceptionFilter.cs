using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.RoatpGateway.Web.Infrastructure;

namespace SFA.DAS.RoatpGateway.Web.Attributes
{
    public class ExternalApiExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is ExternalApiException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectToActionResult("ServiceUnavailable", "RoatpShutterPage", new { });
                filterContext.Result.ExecuteResultAsync(filterContext);
            }
            base.OnException(filterContext);
        }
    }
}
