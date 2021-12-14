using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain.CharityCommission;

namespace SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients
{
    public interface IRoatpApiClient
    {
        Task<CharityDetails> GetCharityDetails(string charityNumber);
    }
}
