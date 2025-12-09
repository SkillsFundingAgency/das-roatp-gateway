using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Infrastructure.Validation;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.Validators
{
    public class RoatpGatewayApplicationViewModelValidator : IRoatpGatewayApplicationViewModelValidator
    {
        private const string NoSelectionErrorMessage = "Select what you want to do";
        private const string ErrorEnterClarificationComments = "Enter your clarification comments";
        private const string ErrorEnterInternalComments = "Enter your internal comments";
        private const string ErrorEnterExternalComments = "Enter your external comments";
        private const string TooManyWords150 = "Your comments must be 150 words or less";
        private const string TooManyWords500 = "Your comments must be 500 words or less";
        private const string ErrorSelectSubcontractingLimit = "Select the applicant's subcontracting limit";

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
                !viewModel.GatewayReviewStatus.Equals(GatewayReviewStatus.Rejected))
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
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionAskClarificationText", TooManyWords150));
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
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedText", TooManyWords150));
                            }
                        }

                        if (string.IsNullOrEmpty(viewModel.OptionFailedExternalText))
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedExternalText", ErrorEnterExternalComments));
                        }
                        else
                        {
                            var wordCount = viewModel.OptionFailedExternalText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 500)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailedExternalText", TooManyWords500));
                            }
                        }
                        break;
                    }
                case GatewayReviewStatus.Rejected:
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
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionRejectedText", TooManyWords150));
                            }
                        }

                        if (string.IsNullOrEmpty(viewModel.OptionExternalRejectedText))
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionExternalRejectedText", ErrorEnterExternalComments));
                        }
                        else
                        {
                            var wordCount = viewModel.OptionExternalRejectedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 500)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionExternalRejectedText", TooManyWords500));
                            }
                        }
                        break;
                    }
                case GatewayReviewStatus.Pass:
                    {
                        if (!string.IsNullOrEmpty(viewModel.OptionApprovedText))
                        {
                            var wordCount = viewModel.OptionApprovedText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                            if (wordCount > 150)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail("OptionApprovedText", TooManyWords150));
                            }
                        }

                        if (viewModel.ApplicationRouteShortText.Equals("Supporting") && !viewModel.SubcontractingLimit.HasValue)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("SubcontractingLimit", ErrorSelectSubcontractingLimit));
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



