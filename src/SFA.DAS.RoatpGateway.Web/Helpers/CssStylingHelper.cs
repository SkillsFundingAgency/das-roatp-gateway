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
                //case SectionReviewStatus.Clarification:
                //    cssModifierClassName = "das-tag--solid-purple";
                //    break;
                case SectionReviewStatus.InProgress:
                default:
                    cssModifierClassName = "";
                    break;
            }

            return cssModifierClassName;
        }
    }
}
