using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpGateway.Web.Settings;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebConfiguration _configuration;

        public HomeController(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications", "RoatpGateway");
        }

        [Route("/Dashboard")]
        public IActionResult Dashboard()
        {
            return Redirect($"{_configuration.EsfaAdminServicesBaseUrl}/Dashboard");
        } 
    }
}
