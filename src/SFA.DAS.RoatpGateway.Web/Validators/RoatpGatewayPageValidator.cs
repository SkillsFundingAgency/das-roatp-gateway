using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Models;
using System.Linq;
using SFA.DAS.RoatpGateway.Web.Validators.Validation;

namespace SFA.DAS.RoatpGateway.Web.Validators
{
    public class RoatpGatewayPageValidator : IRoatpGatewayPageValidator
    {
        private const string ClarificationDetailsRequired = "Enter comments";
        private const string FailDetailsRequired = "Enter comments";
        private const string TooManyWords = "Your comments must be 150 words or less";
        private const string TooManyWordsClarificationAnswer = "Your comments must be {0} words or less";

        private const string ClarificationFile = "ClarificationFile";
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
        private const string MaxFileSizeExceeded = "The selected file must be smaller than 5MB";
        private const string FileMustBePdf = "The selected file must be a PDF";

        public async Task<ValidationResponse> Validate(SubmitGatewayPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (command.OriginalStatus == SectionReviewStatus.Clarification && string.IsNullOrEmpty(command.Status) &&
                command.GatewayReviewStatus == GatewayReviewStatus.InProgress)
                {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass",
                    NoSelectionErrorMessages.Errors[command.PageId]));
                return await Task.FromResult(validationResponse);
            }

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass",
                    NoSelectionErrorMessages.Errors[command.PageId]));
            }
            else
            {
                if (command.Status == SectionReviewStatus.Fail && string.IsNullOrEmpty(command.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        FailDetailsRequired));
                }
                else if (command.Status == SectionReviewStatus.Clarification &&
                         string.IsNullOrEmpty(command.OptionClarificationText))
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
                    var wordCount = command.OptionPassText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                        .Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionPassText",
                            TooManyWords));
                    }

                    break;
                }
                case SectionReviewStatus.Fail when !string.IsNullOrEmpty(command.OptionFailText):
                {
                    var wordCount = command.OptionFailText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                        .Length;
                    if (wordCount > 150)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                            TooManyWords));
                    }

                    break;
                }
                case SectionReviewStatus.InProgress when !string.IsNullOrEmpty(command.OptionInProgressText):
                {
                    var wordCount = command.OptionInProgressText
                        .Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
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
                    var wordCount = command.OptionClarificationText
                        .Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
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
                validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationAnswer",
                    "Add applicant's response"));
            }
            else
            {
                var wordCount = command.ClarificationAnswer.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Length;
                if (wordCount > command.ClarificationAnswerMaxWords)
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationAnswer",
                        string.Format(TooManyWordsClarificationAnswer, command.ClarificationAnswerMaxWords)));
                }
            }

            var otherValidationResults = await Validate(command);

            foreach (var error in otherValidationResults.Errors)
            {
                validationResponse.Errors.Add(error);
            }

            return validationResponse;
        }

        public ValidationResponse ValidateClarificationFileUpload(IFormFileCollection filesToUpload)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            foreach (var file in filesToUpload)
            {

                if (!FileContentIsValidForPdfFile(file))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationFile, FileMustBePdf));

                    break;
                }
                else if (file.Length > MaxFileSizeInBytes)
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationFile,
                        MaxFileSizeExceeded));
                    break;
                }
            }

            if (filesToUpload.Count == 0)
                validationResponse.Errors.Add(new ValidationErrorDetail(ClarificationFile, "Select a file"));


            return validationResponse;
        }



        private static bool FileContentIsValidForPdfFile(IFormFile file)
        {
            var pdfHeader = new byte[] {0x25, 0x50, 0x44, 0x46};

            using (var fileContents = file.OpenReadStream())
            {
                var headerOfActualFile = new byte[pdfHeader.Length];
                fileContents.Read(headerOfActualFile, 0, headerOfActualFile.Length);
                fileContents.Position = 0;

                return headerOfActualFile.SequenceEqual(pdfHeader);
            }
        }
    }

    public interface IRoatpGatewayPageValidator
    {
        Task<ValidationResponse> Validate(SubmitGatewayPageAnswerCommand command);
        Task<ValidationResponse> ValidateClarification(SubmitGatewayPageAnswerCommand command);
        ValidationResponse ValidateClarificationFileUpload(IFormFileCollection filesToUpload);
    }
}
