using System;

namespace SFA.DAS.RoatpGateway.Domain.Roatp;

public class OrganisationRegisterStatus
{
    public bool UkprnOnRegister { get; set; }
    public Guid? OrganisationId { get; set; }
    public int? ProviderTypeId { get; set; }
    public int? StatusId { get; set; }
    public int? RemovedReasonId { get; set; }
    public DateTime? StatusDate { get; set; }

    public static implicit operator OrganisationRegisterStatus(OrganisationResponse response)
    {
        if (response == null)
        {
            return new OrganisationRegisterStatus
            {
                UkprnOnRegister = false
            };
        }
        return new OrganisationRegisterStatus
        {
            UkprnOnRegister = true,
            OrganisationId = response.OrganisationId,
            ProviderTypeId = (int?)response.ProviderType,
            StatusId = (int?)response.Status,
            RemovedReasonId = response.RemovedReasonId,
            StatusDate = response.StatusDate
        };
    }
}
