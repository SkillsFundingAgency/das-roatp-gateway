using SFA.DAS.AdminService.Common.Validation;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpGateway.Web.ViewModels
{
    public abstract class RoatpApplicationActionsViewModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationStatus { get; set; }

        public string Ukprn { get; set; }
        public string ApplyLegalName { get; set; }
        public string ApplicationRoute { get; set; }
        public DateTime? ApplicationSubmittedOn { get; set; }

        public string CssFormGroupError { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public string ConfirmApplicationAction { get; set; }
    }

    public class RoatpWithdrawApplicationViewModel : RoatpApplicationActionsViewModel
    {
        public string CssOnErrorOptionYesText { get; set; }
        public string OptionYesText { get; set; }
    }

    public class RoatpRemoveApplicationViewModel : RoatpApplicationActionsViewModel
    {
        public string CssOnErrorOptionYesText { get; set; }
        public string OptionYesText { get; set; }

        public string CssOnErrorOptionYesTextExternal { get; set; }
        public string OptionYesTextExternal { get; set; }
    }
}