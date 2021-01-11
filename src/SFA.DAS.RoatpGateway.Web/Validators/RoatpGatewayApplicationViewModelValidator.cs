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
        private const string ErrorEnterInternalComments = "Enter your internal comments";
        private const string ErrorEnterExternalComments = "Enter your external comments";
        private const string TooManyWords = "Your comments must be 150 words or less";

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
                !viewModel.GatewayReviewStatus.Equals(GatewayReviewStatus.Pass) &&
                !viewModel.GatewayReviewStatus.Equals(GatewayReviewStatus.Reject))
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
                            if (wordCount > 150)
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
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedText", ErrorEnterInternalComments));
                        }
                        else
                        {
                            var wordCount = viewModel.OptionFailedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 150)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedText", TooManyWords));
                            }
                        }

                        if (string.IsNullOrEmpty(viewModel.OptionFailedExternalText))
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedExternalText", ErrorEnterExternalComments));
                        }
                        else
                        {
                            var wordCount = viewModel.OptionFailedExternalText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 150)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedExternalText", TooManyWords));
                            }
                        }
                        break;
                    }
                case GatewayReviewStatus.Reject:
                {
                    if (string.IsNullOrEmpty(viewModel.OptionRejectedText))
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionRejectedText", ErrorEnterInternalComments));
                    }
                    else
                    {
                        var wordCount = viewModel.OptionRejectedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > 150)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionRejectedText", TooManyWords));
                        }
                    }

                    if (string.IsNullOrEmpty(viewModel.OptionExternalRejectedText))
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionExternalRejectedText", ErrorEnterExternalComments));
                    }
                    else
                    {
                        var wordCount = viewModel.OptionExternalRejectedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > 150)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionExternalRejectedText", TooManyWords));
                        }
                    }
                    break;
                    }
                case GatewayReviewStatus.Pass when !string.IsNullOrEmpty(viewModel.OptionApprovedText):
                    {
                        var wordCount = viewModel.OptionApprovedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > 150)
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



