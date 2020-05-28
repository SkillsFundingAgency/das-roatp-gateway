using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Services
{
    public interface IGatewaySectionsNotRequiredService
    {
        Task SetupNotRequiredLinks(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute);
    }
}
