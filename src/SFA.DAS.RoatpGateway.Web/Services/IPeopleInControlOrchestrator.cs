using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IPeopleInControlOrchestrator
    {
        Task<PeopleInControlPageViewModel> GetPeopleInControlViewModel(GetPeopleInControlRequest getPeopleInControlRequest);
        Task<PeopleInControlHighRiskPageViewModel> GetPeopleInControlHighRiskViewModel(GetPeopleInControlHighRiskRequest getPeopleInControlRequest);
    }
}
