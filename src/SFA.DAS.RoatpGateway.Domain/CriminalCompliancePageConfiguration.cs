using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SFA.DAS.RoatpGateway.Domain
{
    public static class CriminalCompliancePageConfiguration
    {
        private static Dictionary<string, string> headings = new Dictionary<string, string>
        {
            { GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "Composition with creditors check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "Failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "Contract terminated early by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "Withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "Removed from Register of Training Organisations (RoTO) in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "Funding removed from any education bodies in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "Removed from any professional or trade registers in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "Involuntary withdrawal from Initial Teacher Training accreditation in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "Removed from any charity register check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding, "Investigated due to safeguarding issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.InvestigationPublicBody, "Investigated by the ESFA or other public body or regulator check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "Subject to insolvency or winding up proceedings in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions, "Unspent criminal convictions check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds, "Failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities, "Investigated for fraud or irregularities in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation, "Ongoing investigations for fraud or irregularities check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated, "Contract terminated by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract, "Withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments, "Breached tax payments or social security contributions in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees, "Register of Removed Trustees check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt, "People in control or any partner organisations been made bankrupt in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.TeachingRegulations, "Subject to a prohibition order from the Teaching Regulation Agency" },
        { GatewayPageIds.CriminalComplianceWhosInControlChecks.SchoolManagement, "Subject to a ban from management or governance of schools" }
        };

        public static IReadOnlyDictionary<string, string> Headings = new ReadOnlyDictionary<string, string>(headings);
    }
}
