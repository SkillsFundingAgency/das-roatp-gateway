using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Models;

namespace SFA.DAS.RoatpGateway.Web.Validators
{
    public class RoatpGatewayPageValidator : IRoatpGatewayPageValidator
    {
        private const string ClarificationDetailsRequired = "Enter comments";
        private const string FailDetailsRequired = "Enter comments";
        private const string TooManyWords = "Your comments must be 150 words or less";
        private const string TooManyWordsClarificationAnswer = "Your comments must be 300 words or less";

        public async Task<ValidationResponse> Validate(SubmitGatewayPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", NoSelectionErrorMessages.Errors[command.PageId]));
            }
            else
            {
                if (command.Status == SectionReviewStatus.Fail && string.IsNullOrEmpty(command.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        FailDetailsRequired));
                }
                else if (command.Status == SectionReviewStatus.Clarification && string.IsNullOrEmpty(command.OptionClarificationText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionClarificationText",
                        ClarificationDetailsRequired));
                }
            }

            if (validationResponse.Errors.Any())
            {
                return await Task.FromResult(validationResponse);
            }

            switch (command.Status)
            {
                case SectionReviewStatus.Pass when !string.IsNullOrEmpty(command.OptionPassText):
                    {
                        var wordCount = command.OptionPassText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > 150)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionPassText",
                                TooManyWords));
                        }

                        break;
                    }
                case SectionReviewStatus.Fail when !string.IsNullOrEmpty(command.OptionFailText):
                    {
                        var wordCount = command.OptionFailText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > 150)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                                TooManyWords));
                        }

                        break;
                    }
                case SectionReviewStatus.InProgress when !string.IsNullOrEmpty(command.OptionInProgressText):
                    {
                        var wordCount = command.OptionInProgressText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .Length;
                        if (wordCount > 150)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionInProgressText",
                                TooManyWords));
                        }

                        break;
                    }
                case SectionReviewStatus.Clarification when !string.IsNullOrEmpty(command.OptionClarificationText):
                    {
                        var wordCount = command.OptionClarificationText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .Length;
                        if (wordCount > 150)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionClarificationText",
                                TooManyWords));
                        }

                        break;
                    }
            }

            return await Task.FromResult(validationResponse);
        }

        public async Task<ValidationResponse> ValidateClarification(SubmitGatewayPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };
            // var result = await Validate(command);
            if (string.IsNullOrWhiteSpace(command.ClarificationAnswer))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationAnswer", "Add applicant's response"));
            }
            else
            {
                var wordCount = command.ClarificationAnswer.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Length;
                if (wordCount > 300)
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationAnswer",
                        TooManyWordsClarificationAnswer));
                }
            }
            var otherValidationResults = await Validate(command);

            foreach (var error in otherValidationResults.Errors)
            {
                validationResponse.Errors.Add(error);
            }

            return validationResponse;
        }
    }

    public interface IRoatpGatewayPageValidator
    {
        Task<ValidationResponse> Validate(SubmitGatewayPageAnswerCommand command);
        Task<ValidationResponse> ValidateClarification(SubmitGatewayPageAnswerCommand command);
    }
}
