using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpGateway.Web.Validators
{
    public class RoatpGatewayApplicationViewModelValidator : IRoatpGatewayApplicationViewModelValidator
    {
        private const string NoSelectionErrorMessage = "Select what you want to do";
        private const string ErrorEnterClarificationComments = "Enter your clarification comments";
        private const string ErrorEnterDeclinedComments = "Enter your comments";
        private const string TooManyWords = "Your comments must be 500 words or less";

        public async Task<ValidationResponse> Validate(RoatpGatewayApplicationViewModel viewModel)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrEmpty(viewModel.GatewayReviewStatus) ||
                !string.IsNullOrEmpty(viewModel.GatewayReviewStatus) &&
                !viewModel.GatewayReviewStatus.Equals(GatewayReviewStatus.ClarificationSent) &&
                !viewModel.GatewayReviewStatus.Equals(GatewayReviewStatus.Fail) &&
                !viewModel.GatewayReviewStatus.Equals(GatewayReviewStatus.Pass))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("GatewayReviewStatus", NoSelectionErrorMessage));
            }

            if (validationResponse.Errors.Any()) return await Task.FromResult(validationResponse);


            switch (viewModel.GatewayReviewStatus)
            {
                case GatewayReviewStatus.ClarificationSent:
                    {
                        if (string.IsNullOrEmpty(viewModel.OptionAskClarificationText))
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionAskClarificationText", ErrorEnterClarificationComments));
                        }
                        else
                        {
                            var wordCount = viewModel.OptionAskClarificationText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 500)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionAskClarificationText", TooManyWords));
                            }
                        }


                        break;
                    }
                case GatewayReviewStatus.Fail:
                    {
                        if (string.IsNullOrEmpty(viewModel.OptionFailedText))
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedText", ErrorEnterDeclinedComments));
                        }
                        else
                        {
                            var wordCount = viewModel.OptionFailedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 500)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedText", TooManyWords));
                            }
                        }
                        break;
                    }
                case GatewayReviewStatus.Pass when !string.IsNullOrEmpty(viewModel.OptionApprovedText):
                    {
                        var wordCount = viewModel.OptionApprovedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > 500)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionApprovedText", TooManyWords));
                        }

                        break;
                    }
            }

            return await Task.FromResult(validationResponse);
        }
    }

    public interface IRoatpGatewayApplicationViewModelValidator
    {
        Task<ValidationResponse> Validate(RoatpGatewayApplicationViewModel viewModel);

    }
}



