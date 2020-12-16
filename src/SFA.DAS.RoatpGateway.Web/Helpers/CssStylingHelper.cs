using SFA.DAS.RoatpGateway.Domain;

namespace SFA.DAS.RoatpGateway.Web.Helpers
{
    public static class CssStylingHelper
    {
        public static string GetSectionReviewStatusStyle(string status)
        {
            string cssModifierClassName;

            switch (status)
            {
                case SectionReviewStatus.NotRequired:
                    cssModifierClassName = "govuk-tag--inactive";
                    break;
                case SectionReviewStatus.Pass:
                    cssModifierClassName = "das-tag--solid-green";
                    break;
                case SectionReviewStatus.Fail:
                    cssModifierClassName = "das-tag--solid-red";
                    break;
                case SectionReviewStatus.Clarification:
                    cssModifierClassName = "das-tag--solid-purple";
                    break;
                default:
                    cssModifierClassName = "";
                    break;
            }

            return cssModifierClassName;
        }

        public static string GetGatewayReviewStatusStyle(string status)
        {
            string cssModifierClassName;

            switch (status)
            {
                case GatewayReviewStatus.Pass:
                    cssModifierClassName = "das-tag--solid-green";
                    break;
                case GatewayReviewStatus.Fail:
                    cssModifierClassName = "das-tag--solid-red";
                    break;
                case GatewayReviewStatus.Reject:
                    cssModifierClassName = "das-tag--solid-brown";
                    break;
                case GatewayReviewStatus.ClarificationSent:
                    cssModifierClassName = "das-tag--solid-purple";
                    break;
                default:
                    cssModifierClassName = "";
                    break;
            }

            return cssModifierClassName;
        }
    }
}
