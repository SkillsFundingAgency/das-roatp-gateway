using System;

namespace SFA.DAS.RoatpGateway.Domain.Roatp;

public class OrganisationResponse
{
    public Guid OrganisationId { get; set; }
    public int Ukprn { get; set; }
    public ProviderType ProviderType { get; set; }
    public string RemovedReason { get; set; }
    public OrganisationStatus Status { get; set; }
    public DateTime StatusDate { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public string CompanyNumber { get; set; }
    public string CharityNumber { get; set; }
    public int OrganisationTypeId { get; set; }
    public string OrganisationType { get; set; }
    public DateTime? ApplicationDeterminedDate { get; set; }
    public int? RemovedReasonId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}

