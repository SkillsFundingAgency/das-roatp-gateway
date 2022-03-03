using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ModelBinders;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class RoatpGatewayController : RoatpGatewayControllerBase<RoatpGatewayController>
    {
        private readonly IGatewayOverviewOrchestrator _orchestrator;
        private readonly IRoatpGatewayApplicationViewModelValidator _validator;
        private readonly IRoatpSearchTermValidator _searchValidator;

        public RoatpGatewayController(IRoatpApplicationApiClient applyApiClient,
                                     IGatewayOverviewOrchestrator orchestrator, IRoatpGatewayApplicationViewModelValidator validator,
                                     IRoatpSearchTermValidator searchValidator,
                                     ILogger<RoatpGatewayController> logger, IRoatpGatewayPageValidator gatewayValidator)
            :base(applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
            _validator = validator;
            _searchValidator = searchValidator;
        }

        [HttpGet("/Roatp/Gateway/New")]
        public async Task<IActionResult> NewApplications([StringTrim] string searchTerm, string sortOrder, int page = 1)
        {
            if(searchTerm != null)
            {
                var validationResponse = _searchValidator.Validate(searchTerm);

                foreach(var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var applications = await _applyApiClient.GetNewGatewayApplications(ModelState.IsValid ? searchTerm : null, sortOrder);
            var applicationCounts = await _applyApiClient.GetApplicationCounts(ModelState.IsValid ? searchTerm : null);

            var viewModel = new RoatpGatewayDashboardViewModel
            {
                Applications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue),
                ApplicationCounts = applicationCounts,
                SelectedTab = nameof(NewApplications),
                SearchTerm = searchTerm,
                SortColumn = null,
                SortOrder = sortOrder
            };

            return View("~/Views/Gateway/NewApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/InProgress")]
        public async Task<IActionResult> InProgressApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder, int page = 1)
        {
            if (searchTerm != null)
            {
                var validationResponse = _searchValidator.Validate(searchTerm);

                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var applications = await _applyApiClient.GetInProgressGatewayApplications(ModelState.IsValid ? searchTerm : null, sortColumn, sortOrder);
            var applicationCounts = await _applyApiClient.GetApplicationCounts(ModelState.IsValid ? searchTerm : null);

            var viewModel = new RoatpGatewayDashboardViewModel
            {
                Applications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue),
                ApplicationCounts = applicationCounts,
                SelectedTab = nameof(InProgressApplications),
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortOrder = sortOrder
            };

            return View("~/Views/Gateway/InProgressApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/Closed")]
        public async Task<IActionResult> ClosedApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder, int page = 1)
        {
            if (searchTerm != null)
            {
                var validationResponse = _searchValidator.Validate(searchTerm);

                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var applications = await _applyApiClient.GetClosedGatewayApplications(ModelState.IsValid ? searchTerm : null, sortColumn, sortOrder);
            var applicationCounts = await _applyApiClient.GetApplicationCounts(ModelState.IsValid ? searchTerm : null);

            var viewModel = new RoatpGatewayDashboardViewModel
            {
                Applications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue),
                ApplicationCounts = applicationCounts,
                SelectedTab = nameof(ClosedApplications),
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortOrder = sortOrder
            };

            return View("~/Views/Gateway/ClosedApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();

            var viewModel =
                    await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));
            
            if (viewModel is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }
            else if (viewModel.ApplicationStatus == ApplicationStatus.Removed
                    || viewModel.ApplicationStatus == ApplicationStatus.Withdrawn)
            {
                return View("~/Views/Gateway/Application_Closed.cshtml", viewModel);
            }
            else
            {
                switch (viewModel.GatewayReviewStatus)
                {
                    case GatewayReviewStatus.New:
                    case GatewayReviewStatus.InProgress:
                    case GatewayReviewStatus.ClarificationSent:
                        return View("~/Views/Gateway/Application.cshtml", viewModel);
                    case GatewayReviewStatus.Pass:
                    case GatewayReviewStatus.Fail:
                    case GatewayReviewStatus.Rejected:
                        return View("~/Views/Gateway/Application_ReadOnly.cshtml", viewModel);
                    default:
                        return RedirectToAction(nameof(NewApplications));
                }
            }
        }



        [HttpGet("/Roatp/Gateway/Clarification/{applicationId}")]
        public async Task<IActionResult> AskForClarification(Guid applicationId)
        {
            var username = HttpContext.User.UserDisplayName();

            var viewModel =
                await _orchestrator.GetClarificationViewModel(new GetApplicationClarificationsRequest(applicationId, username));
            
            
            if (viewModel is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            return View("~/Views/Gateway/AskForClarification.cshtml", viewModel);
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/AboutToAskForClarification")]
        public async Task<IActionResult> AboutToAskForClarification(Guid applicationId, string confirmAskForClarification)
        {
            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetClarificationViewModel(new GetApplicationClarificationsRequest(applicationId, username));

            if (viewModel is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }
            else if (string.IsNullOrEmpty(confirmAskForClarification))
            {
                viewModel.ErrorMessages = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail("ConfirmAskForClarification",
                        "Select if you are sure you want to ask for clarification")
                };

                viewModel.CssFormGroupError = "govuk-form-group--error";
                return View("~/Views/Gateway/AskForClarification.cshtml", viewModel);
            }
            else if (confirmAskForClarification == "No" || viewModel.GatewayReviewStatus == GatewayReviewStatus.ClarificationSent)
            {
                return RedirectToAction(nameof(ViewApplication), new { applicationId });
            }

            var userId = HttpContext.User.UserId();
            await _applyApiClient.UpdateGatewayReviewStatusAsClarification(applicationId, userId, username);
            var vm = new RoatpGatewayOutcomeViewModel { ApplicationId = applicationId};
            return View("~/Views/Gateway/ConfirmApplicationClarification.cshtml", vm);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/ConfirmOutcome")]
        public async Task<IActionResult> ConfirmOutcome(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment, string gatewayReviewExternalComment, int? subcontractingLimit)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            var username = HttpContext.User.UserDisplayName();
            var viewModel = await _orchestrator.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));

            if (viewModel.ReadyToConfirm)
            {
                switch (gatewayReviewStatus)
                {
                    case GatewayReviewStatus.ClarificationSent:
                        {
                            viewModel.RadioCheckedAskClarification = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionAskClarificationText = gatewayReviewComment;
                            break;
                        }
                    case GatewayReviewStatus.Fail:
                        {
                            viewModel.RadioCheckedFailed = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionFailedText = gatewayReviewComment;
                            viewModel.OptionFailedExternalText = gatewayReviewExternalComment;
                            break;
                        }
                    case GatewayReviewStatus.Pass:
                        {
                            if(subcontractingLimit == 100000)
                            {
                                viewModel.RadioChecked100kSubcontractingLimit = HtmlAndCssElements.CheckBoxChecked;
                            }
                            else if (subcontractingLimit == 500000)
                            {
                                viewModel.RadioChecked500kSubcontractingLimit = HtmlAndCssElements.CheckBoxChecked;
                            }
                            viewModel.RadioCheckedApproved = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionApprovedText = gatewayReviewComment;
                            break;
                        }
                    case GatewayReviewStatus.Rejected:
                    {
                        viewModel.RadioCheckedRejected = HtmlAndCssElements.CheckBoxChecked;
                        viewModel.OptionRejectedText = gatewayReviewComment;
                        viewModel.OptionExternalRejectedText = gatewayReviewExternalComment;
                        break;
                    }
                }

                if (viewModel.ApplicationStatus == ApplicationStatus.GatewayAssessed)
                {
                    return RedirectToAction(nameof(NewApplications));
                }
                else
                {
                    return View("~/Views/Gateway/ConfirmOutcome.cshtml", viewModel);
                }
            }
            else
            {
                return RedirectToAction(nameof(ViewApplication), new { applicationId });
            }
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/ConfirmOutcome")]
        public async Task<IActionResult> EvaluateConfirmOutcome(RoatpGatewayApplicationViewModel viewModel)
        {
            var application = await _applyApiClient.GetApplication(viewModel.ApplicationId);

            if (application?.ApplyData?.ApplyDetails is null || application.ApplicationStatus == ApplicationStatus.GatewayAssessed)
            {
                return RedirectToAction(nameof(NewApplications));
            }
            else
            {
                viewModel.ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
            }

            var validationResponse = await _validator.Validate(viewModel);

            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                var username = HttpContext.User.UserDisplayName();
                var viewModelOnError = await _orchestrator.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(viewModel.ApplicationId, username));
                if (viewModelOnError != null)
                {
                    _orchestrator.ProcessViewModelOnError(viewModelOnError, viewModel, validationResponse);
                    return View("~/Views/Gateway/ConfirmOutcome.cshtml", viewModelOnError);
                }
                else
                {
                    return RedirectToAction(nameof(ViewApplication), new { applicationId = viewModel.ApplicationId });
                }
            }

            var viewName = "~/Views/Gateway/ConfirmOutcomeAskClarification.cshtml";


            var confirmViewModel = new RoatpGatewayOutcomeViewModel { ApplicationId = viewModel.ApplicationId, GatewayReviewStatus = viewModel.GatewayReviewStatus };

            switch (viewModel.GatewayReviewStatus)
            {
                case GatewayReviewStatus.ClarificationSent:
                    {
                        confirmViewModel.GatewayReviewComment = viewModel.OptionAskClarificationText;
                        break;
                    }
                case GatewayReviewStatus.Fail:
                    {
                        confirmViewModel = new RoatpGatewayFailOutcomeViewModel
                        {
                            ApplicationId = viewModel.ApplicationId,
                            GatewayReviewStatus = viewModel.GatewayReviewStatus,
                            GatewayReviewComment = viewModel.OptionFailedText,
                            GatewayReviewExternalComment = viewModel.OptionFailedExternalText
                        };
                        viewName = "~/Views/Gateway/ConfirmOutcomeFailed.cshtml";
                        break;
                    }
                case GatewayReviewStatus.Pass:
                    {
                        confirmViewModel = new RoatpGatewayConfirmOutcomeViewModel
                        {
                            ApplicationId = viewModel.ApplicationId,
                            GatewayReviewStatus = viewModel.GatewayReviewStatus,
                            GatewayReviewComment = viewModel.OptionApprovedText,
                            GatewayReviewExternalComment = string.Empty,
                            SubcontractingLimit = viewModel.SubcontractingLimit
                        };
                        viewName = "~/Views/Gateway/ConfirmOutcomeApproved.cshtml";
                        break;
                    }
                case GatewayReviewStatus.Rejected:
                {
                    var contact = await _applyApiClient.GetContactDetails(viewModel.ApplicationId);
                    confirmViewModel = new RoatpGatewayRejectedOutcomeViewModel
                    {
                        ApplicationId = viewModel.ApplicationId,
                        Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                        ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                        ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                        ApplicationReferenceNumber = application.ApplyData.ApplyDetails.ReferenceNumber,
                        ApplicationEmailAddress = contact?.Email,
                        ApplicationSubmittedOn = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                        GatewayReviewStatus = viewModel.GatewayReviewStatus,
                        GatewayReviewComment = viewModel.OptionRejectedText,
                        GatewayReviewExternalComment = viewModel.OptionExternalRejectedText
                    };
                    viewName = "~/Views/Gateway/ConfirmOutcomeRejected.cshtml";
                    break;
                }
            }

            return View(viewName, confirmViewModel);
            
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/AboutToConfirmOutcome")]
        public async Task<IActionResult> AboutToConfirmOutcome(RoatpGatewayConfirmOutcomeViewModel viewModel)
        {
            if (viewModel.ApplicationStatus == ApplicationStatus.GatewayAssessed)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            if (ModelState.IsValid)
            {
                if (viewModel.ConfirmGatewayOutcome.Equals(HtmlAndCssElements.RadioButtonValueYes, StringComparison.InvariantCultureIgnoreCase))
                {
                    var username = HttpContext.User.UserDisplayName();
                    var userId = HttpContext.User.UserId();
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, string.Empty, viewModel.SubcontractingLimit, userId, username);
                    var vm = new RoatpGatewayOutcomeViewModel { GatewayReviewStatus = viewModel.GatewayReviewStatus };
                    return View("~/Views/Gateway/GatewayOutcomeConfirmation.cshtml", vm);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new
                    {
                        applicationId = viewModel.ApplicationId,
                        gatewayReviewStatus = viewModel.GatewayReviewStatus,
                        gatewayReviewComment = viewModel.GatewayReviewComment,
                        gatewayReviewExternalComments = viewModel.GatewayReviewExternalComment,
                        subcontractingLimit = viewModel.SubcontractingLimit
                    });
                }
            }
            else
            {
                viewModel.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                return View("~/Views/Gateway/ConfirmOutcomeApproved.cshtml", viewModel);
            }
        }


        [HttpPost("/Roatp/Gateway/{applicationId}/AboutToFailOutcome")]
        public async Task<IActionResult> AboutToFailOutcome(RoatpGatewayFailOutcomeViewModel viewModel)
        {
            if (viewModel.ApplicationStatus == ApplicationStatus.GatewayAssessed)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            if (ModelState.IsValid)
            {
                if (viewModel.ConfirmGatewayOutcome.Equals(HtmlAndCssElements.RadioButtonValueYes, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = HttpContext.User.UserId();
                    var username = HttpContext.User.UserDisplayName();
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, userId, username);
                    
                    var vm = new RoatpGatewayOutcomeViewModel {GatewayReviewStatus = viewModel.GatewayReviewStatus};
                    return View("~/Views/Gateway/GatewayOutcomeConfirmation.cshtml", vm);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new
                    {
                        applicationId = viewModel.ApplicationId,
                        gatewayReviewStatus = viewModel.GatewayReviewStatus,
                        gatewayReviewComment = viewModel.GatewayReviewComment,
                        gatewayReviewExternalComment = viewModel.GatewayReviewExternalComment
                    });
                }
            }
            else
            {
                viewModel.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                return View("~/Views/Gateway/ConfirmOutcomeFailed.cshtml", viewModel);
            }
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/AboutToRejectOutcome")]
        public async Task<IActionResult> AboutToRejectOutcome(RoatpGatewayRejectedOutcomeViewModel viewModel)
        {
            if (viewModel.ApplicationStatus == ApplicationStatus.GatewayAssessed)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            if (ModelState.IsValid)
            {
                if (viewModel.ConfirmGatewayOutcome.Equals(HtmlAndCssElements.RadioButtonValueYes, StringComparison.InvariantCultureIgnoreCase))
                {
                    var username = HttpContext.User.UserDisplayName();
                    var userId = HttpContext.User.UserId();
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, viewModel.GatewayReviewExternalComment, viewModel.SubcontractingLimit, userId, username);

                    var vm = new RoatpGatewayOutcomeViewModel { GatewayReviewStatus = viewModel.GatewayReviewStatus };
                    return View("~/Views/Gateway/ApplicationRejected.cshtml", vm);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new
                    {
                        applicationId = viewModel.ApplicationId,
                        gatewayReviewStatus = viewModel.GatewayReviewStatus,
                        gatewayReviewComment = viewModel.GatewayReviewComment,
                        gatewayReviewExternalComment = viewModel.GatewayReviewExternalComment
                    });
                }
            }
            else
            {
                viewModel.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                return View("~/Views/Gateway/ConfirmOutcomeRejected.cshtml", viewModel);
            }
        }

        [HttpPost("/Roatp/Gateway")]
        public async Task<IActionResult> EvaluateGateway(RoatpGatewayApplicationViewModel viewModel, bool? isGatewayApproved)
        {
            var application = await _applyApiClient.GetApplication(viewModel.ApplicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            if (!isGatewayApproved.HasValue)
            {
                ModelState.AddModelError("IsGatewayApproved", "Please evaluate Gateway");
            }

            if (ModelState.IsValid)
            {
                var username = HttpContext.User.UserDisplayName();
                var userId = HttpContext.User.UserId();
                await _applyApiClient.EvaluateGateway(viewModel.ApplicationId, isGatewayApproved.Value, userId, username);
                return RedirectToAction(nameof(Evaluated), new { viewModel.ApplicationId });
            }
            else
            {
                var username = HttpContext.User.UserDisplayName();
                var newViewModel = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(application.ApplicationId, username));
                return View("~/Views/Gateway/Application.cshtml", newViewModel);
            }
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(NewApplications));
            }

            return View("~/Views/Gateway/Evaluated.cshtml");
        }
    }
}