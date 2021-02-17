using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IGatewayRegisterChecksOrchestrator
    {
        Task<RoatpPageViewModel> GetRoatpViewModel(GetRoatpRequest request);
        Task<RoepaoPageViewModel> GetRoepaoViewModel(GetRoepaoRequest request);
    }
}
