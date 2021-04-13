using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpGateway.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Attributes;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.Domain;

namespace SFA.DAS.RoatpGateway.Web.Controllers
{
    [ExternalApiExceptionFilter]
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public abstract class RoatpGatewayControllerBase<T> : Controller
    {
        protected readonly IRoatpApplicationApiClient _applyApiClient;
        protected readonly ILogger<T> _logger;
        protected readonly IRoatpGatewayPageValidator GatewayValidator;
        protected const string GatewayViewsLocation = "~/Views/Gateway/pages";

        public RoatpGatewayControllerBase(IRoatpApplicationApiClient applyApiClient,
                                          ILogger<T> logger, IRoatpGatewayPageValidator gatewayValidator)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            GatewayValidator = gatewayValidator;
        }

        protected async Task<IActionResult> ValidateAndUpdatePageAnswer<VM>(SubmitGatewayPageAnswerCommand command,
                                                                  Func<Task<VM>> viewModelBuilder,
                                                                  string errorView) where VM : RoatpGatewayPageViewModel
        {
            var validationResponse = await GatewayValidator.Validate(command);
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionClarificationText = command.OptionClarificationText;
                viewModel.OptionPassText = command.OptionPassText;
                viewModel.ErrorMessages = validationResponse.Errors;
                return View(errorView, viewModel);
            }

            if (command.Status == SectionReviewStatus.Clarification)
            {
                return await SubmitGatewayPageAnswerClarification(command);
            }
            else
            {
                return await SubmitGatewayPageAnswer(command);
            }
        }


        protected async Task<IActionResult> ValidateAndUpdateClarificationPageAnswer<VM>(SubmitGatewayPageAnswerCommand command,
            Func<Task<VM>> viewModelBuilder,
            string errorView) where VM : RoatpGatewayPageViewModel
        {
            var validationResponse = await GatewayValidator.ValidateClarification(command);
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionClarificationText = command.OptionClarificationText;
                viewModel.OptionPassText = command.OptionPassText;
                viewModel.ClarificationAnswer = command.ClarificationAnswer;
                viewModel.ErrorMessages = validationResponse.Errors;
                
                return View(errorView, viewModel);
            }

            return await SubmitGatewayPageAnswerPostClarification(command);
        }

        protected async Task<IActionResult> SubmitGatewayPageAnswer(SubmitGatewayPageAnswerCommand command)
        {
            var username = HttpContext.User.UserDisplayName();
            var userId = HttpContext.User.UserId();
            var comments = GetCommentsFromCommand(command);
            var clarificationAnswer = command.ClarificationAnswer;

            _logger.LogInformation($"{typeof(T).Name}-SubmitGatewayPageAnswer - ApplicationId '{command.ApplicationId}' - PageId '{command.PageId}' - Status '{command.Status}' - UserName '{username}' - Comments '{comments}' - ClarificationAnswer {clarificationAnswer}");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(command.ApplicationId, command.PageId, command.Status, userId, username, comments, clarificationAnswer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(T).Name}-SubmitGatewayPageAnswer - Error: '{ex.Message}'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { command.ApplicationId }, $"Sequence_{command.SequenceNumber}");
        }

        protected async Task<IActionResult> SubmitGatewayPageAnswerClarification(SubmitGatewayPageAnswerCommand command)
        {
            var username = HttpContext.User.UserDisplayName();
            var userId = HttpContext.User.UserId();
            var comments = GetCommentsFromCommand(command);
            var clarificationAnswer = command.ClarificationAnswer;

            _logger.LogInformation($"{typeof(T).Name}-SubmitGatewayPageAnswerClarification - ApplicationId '{command.ApplicationId}' - PageId '{command.PageId}' - Status '{command.Status}' - UserName '{username}' - Comments '{comments}' - ClarificationAnswer {clarificationAnswer}");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswerClarification(command.ApplicationId, command.PageId, command.Status, userId, username, comments, clarificationAnswer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(T).Name}-SubmitGatewayPageAnswerClarification - Error: '{ex.Message}'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { command.ApplicationId }, $"Sequence_{command.SequenceNumber}");
        }


        protected async Task<IActionResult> SubmitGatewayPageAnswerPostClarification(SubmitGatewayPageAnswerCommand command)
        {
            var username = HttpContext.User.UserDisplayName();
            var userId = HttpContext.User.UserId();
            var comments = GetCommentsFromCommand(command);
            var clarificationAnswer = command.ClarificationAnswer;

            _logger.LogInformation($"{typeof(T).Name}-SubmitGatewayPageAnswerPostClarification - ApplicationId '{command.ApplicationId}' - PageId '{command.PageId}' - Status '{command.Status}' - UserName '{username}' - Comments '{comments}' - ClarificationAnswer {clarificationAnswer}");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswerPostClarification(command.ApplicationId, command.PageId, command.Status, userId, username, comments, clarificationAnswer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(T).Name}-SubmitGatewayPageAnswerPostClarification - Error: '{ex.Message}'");
                throw;
            }

            return RedirectToAction("ViewApplication", "RoatpGateway", new { command.ApplicationId }, $"Sequence_{command.SequenceNumber}");
        }

        private static string GetCommentsFromCommand(SubmitGatewayPageAnswerCommand command)
        {
            if (command is null) return null;

            switch (command.Status)
            {
                case SectionReviewStatus.Pass:
                    return !string.IsNullOrEmpty(command.OptionPassText) ? command.OptionPassText : null;
                case SectionReviewStatus.Fail:
                    return !string.IsNullOrEmpty(command.OptionFailText) ? command.OptionFailText : null;
                case SectionReviewStatus.InProgress:
                    return !string.IsNullOrEmpty(command.OptionInProgressText) ? command.OptionInProgressText : null;
                case SectionReviewStatus.Clarification:
                    return !string.IsNullOrEmpty(command.OptionClarificationText) ? command.OptionClarificationText : null;
                default:
                    return null;
            }
        }
    }
}
