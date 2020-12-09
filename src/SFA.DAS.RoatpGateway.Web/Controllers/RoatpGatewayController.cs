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
using SFA.DAS.RoatpGateway.Domain.Apply;
using SFA.DAS.RoatpGateway.Web.Services;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    public class RoatpGatewayController : RoatpGatewayControllerBase<RoatpGatewayController>
    {
        private readonly IGatewayOverviewOrchestrator _orchestrator;
        private readonly IRoatpGatewayApplicationViewModelValidator _validator;

        public RoatpGatewayController(IRoatpApplicationApiClient applyApiClient,
                                     IGatewayOverviewOrchestrator orchestrator, IRoatpGatewayApplicationViewModelValidator validator, 
                                     ILogger<RoatpGatewayController> logger, IRoatpGatewayPageValidator gatewayValidator)
            :base(applyApiClient, logger, gatewayValidator)
        {
            _orchestrator = orchestrator;
            _validator = validator;
        }

        [HttpGet("/Roatp/Gateway/New")]
        public async Task<IActionResult> NewApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetNewGatewayApplications();
            var counts = await _applyApiClient.GetApplicationCounts();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewModel = new RoatpGatewayDashboardViewModel
            {
                Applications = paginatedApplications,
                ApplicationCounts = counts,
                SelectedTab = nameof(NewApplications)
            };

            return View("~/Views/Gateway/NewApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/InProgress")]
        public async Task<IActionResult> InProgressApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetInProgressGatewayApplications();
            var counts = await _applyApiClient.GetApplicationCounts();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewModel = new RoatpGatewayDashboardViewModel
            {
                Applications = paginatedApplications,
                ApplicationCounts = counts,
                SelectedTab = nameof(InProgressApplications)
            };

            return View("~/Views/Gateway/InProgressApplications.cshtml", viewModel);
        }

        [HttpGet("/Roatp/Gateway/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedGatewayApplications();
            var counts = await _applyApiClient.GetApplicationCounts();

            var paginatedApplications = new PaginatedList<RoatpApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewModel = new RoatpGatewayDashboardViewModel
            {
                Applications = paginatedApplications,
                ApplicationCounts = counts,
                SelectedTab = nameof(ClosedApplications)
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

            switch (viewModel.GatewayReviewStatus)
            {
                case GatewayReviewStatus.New:
                case GatewayReviewStatus.InProgress:
                    return View("~/Views/Gateway/Application.cshtml", viewModel);
                case GatewayReviewStatus.Pass:
                case GatewayReviewStatus.Fail:
                    return View("~/Views/Gateway/Application_ReadOnly.cshtml", viewModel);
                default:
                    return RedirectToAction(nameof(NewApplications));
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

        // private static Apply GetApplicationData(RoatpApplicationResponse application)
        // {
        //     return new Apply
        //     {
        //         ApplyData = new RoatpApplyData
        //         {
        //             ApplyDetails = new RoatpApplyDetails
        //             {
        //                 ReferenceNumber = application.ApplyData.ApplyDetails.ReferenceNumber,
        //                 ProviderRoute = application.ApplyData.ApplyDetails.ProviderRoute,
        //                 ProviderRouteName = application.ApplyData.ApplyDetails.ProviderRouteName,
        //                 UKPRN = application.ApplyData.ApplyDetails.UKPRN,
        //                 OrganisationName = application.ApplyData.ApplyDetails.OrganisationName,
        //                 ApplicationSubmittedOn = application.ApplyData.ApplyDetails.ApplicationSubmittedOn
        //             }
        //         },
        //         Id = application.Id,
        //         ApplicationId = application.ApplicationId,
        //         OrganisationId = application.OrganisationId,
        //         ApplicationStatus = application.ApplicationStatus,
        //         GatewayReviewStatus = application.GatewayReviewStatus,
        //         AssessorReviewStatus = application.AssessorReviewStatus,
        //         FinancialReviewStatus = application.FinancialReviewStatus
        //     };
        // }


        [HttpPost("/Roatp/Gateway/{applicationId}/AboutToAskForClarification")]
        public async Task<IActionResult> AboutToAskForClarification(Guid applicationId, string confirmAskForClarification)
        {

            var username = HttpContext.User.UserDisplayName();

            if (string.IsNullOrEmpty(confirmAskForClarification))
            {
                var viewModel = await _orchestrator.GetClarificationViewModel(new GetApplicationClarificationsRequest(applicationId, username));
                if (viewModel is null)
                {
                    return RedirectToAction(nameof(NewApplications));
                }
                viewModel.ErrorMessages = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail("ConfirmAskForClarification",
                        "Select if you are sure you want to ask for clarification")
                };
                return View("~/Views/Gateway/AskForClarification.cshtml", viewModel);
            }

            if (confirmAskForClarification == "No")
            {
                var viewModel =
                    await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));
                
                if (viewModel is null)
                {
                    return RedirectToAction(nameof(NewApplications));
                }
                return View("~/Views/Gateway/Application.cshtml", viewModel);
            }
            if (true)
            {
                return RedirectToAction(nameof(NewApplications));
            }
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/ConfirmOutcome")]
        public async Task<IActionResult> ConfirmOutcome(Guid applicationId, string gatewayReviewStatus, string gatewayReviewComment)
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
                            break;
                        }
                    case GatewayReviewStatus.Pass:
                        {
                            viewModel.RadioCheckedApproved = HtmlAndCssElements.CheckBoxChecked;
                            viewModel.OptionApprovedText = gatewayReviewComment;
                            break;
                        }
                    case GatewayReviewStatus.Reject:
                    {
                        viewModel.RadioCheckedRejected = HtmlAndCssElements.CheckBoxChecked;
                        viewModel.OptionRejectedText = gatewayReviewComment;
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

            if (application is null || application.ApplicationStatus == ApplicationStatus.GatewayAssessed)
            {
                return RedirectToAction(nameof(NewApplications));
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
                            GatewayReviewComment = viewModel.OptionFailedText
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
                            GatewayReviewComment = viewModel.OptionApprovedText
                        };
                        viewName = "~/Views/Gateway/ConfirmOutcomeApproved.cshtml";
                        break;
                    }
                case GatewayReviewStatus.Reject:
                {
                    confirmViewModel = new RoatpGatewayRejectOutcomeViewModel
                    {
                        ApplicationId = viewModel.ApplicationId,
                        GatewayReviewStatus = viewModel.GatewayReviewStatus,
                        GatewayReviewComment = viewModel.OptionRejectedText
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
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, userId, username);
                    var vm = new RoatpGatewayOutcomeViewModel { GatewayReviewStatus = viewModel.GatewayReviewStatus };
                    return View("~/Views/Gateway/GatewayOutcomeConfirmation.cshtml", vm);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new
                    {
                        applicationId = viewModel.ApplicationId,
                        gatewayReviewStatus = viewModel.GatewayReviewStatus,
                        gatewayReviewComment = viewModel.GatewayReviewComment
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
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, userId, username);
                    
                    var vm = new RoatpGatewayOutcomeViewModel {GatewayReviewStatus = viewModel.GatewayReviewStatus};
                    return View("~/Views/Gateway/GatewayOutcomeConfirmation.cshtml", vm);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new
                    {
                        applicationId = viewModel.ApplicationId,
                        gatewayReviewStatus = viewModel.GatewayReviewStatus,
                        gatewayReviewComment = viewModel.GatewayReviewComment
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
        public async Task<IActionResult> AboutToRejectOutcome(RoatpGatewayRejectOutcomeViewModel viewModel)
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
                    await _applyApiClient.UpdateGatewayReviewStatusAndComment(viewModel.ApplicationId, viewModel.GatewayReviewStatus, viewModel.GatewayReviewComment, userId, username);

                    var vm = new RoatpGatewayOutcomeViewModel { GatewayReviewStatus = viewModel.GatewayReviewStatus };
                    return View("~/Views/Gateway/GatewayOutcomeConfirmation.cshtml", vm);
                }
                else
                {
                    return RedirectToAction(nameof(ConfirmOutcome), new
                    {
                        applicationId = viewModel.ApplicationId,
                        gatewayReviewStatus = viewModel.GatewayReviewStatus,
                        gatewayReviewComment = viewModel.GatewayReviewComment
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
                await _applyApiClient.EvaluateGateway(viewModel.ApplicationId, isGatewayApproved.Value, HttpContext.User.UserDisplayName());
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


        [HttpGet("/Roatp/Gateway/{applicationId}/Remove")]
        public async Task<IActionResult> RemoveApplication(Guid applicationId)
        {
            // placeholder for now
            await Task.CompletedTask;
            var vm = new RoatpGatewayOutcomeViewModel { ApplicationId = applicationId };
            return View("~/Views/Gateway/ConfirmRemoveApplication.cshtml", vm);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Remove")]
        public async Task<IActionResult> ConfirmRemoveApplication(Guid applicationId)
        {
            // placeholder for now
            await Task.CompletedTask;
            var vm = new RoatpGatewayOutcomeViewModel { ApplicationId = applicationId };
            return View("~/Views/Gateway/ConfirmRemoveApplication.cshtml", vm);
        }

        [HttpGet("/Roatp/Gateway/{applicationId}/Withdraw")]
        public async Task<IActionResult> WithdrawApplication(Guid applicationId)
        {
            // placeholder for now
            await Task.CompletedTask;
            var vm = new RoatpGatewayOutcomeViewModel { ApplicationId = applicationId };
            return View("~/Views/Gateway/ConfirmWithdrawApplication.cshtml", vm);
        }

        [HttpPost("/Roatp/Gateway/{applicationId}/Withdraw")]
        public async Task<IActionResult> ConfirmWithdrawApplication(Guid applicationId)
        {
            // placeholder for now
            await Task.CompletedTask;
            var vm = new RoatpGatewayOutcomeViewModel { ApplicationId = applicationId };
            return View("~/Views/Gateway/ConfirmWithdrawApplication.cshtml", vm);
        }
    }
}