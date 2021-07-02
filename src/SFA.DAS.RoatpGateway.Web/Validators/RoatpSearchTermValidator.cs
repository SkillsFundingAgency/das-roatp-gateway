using System.Collections.Generic;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.RoatpGateway.Web.Validators
{
    public class RoatpSearchTermValidator : IRoatpSearchTermValidator
    {
        private const string SearchTerm = "SearchTerm";
        private const int MinimumSearchTermLength = 3;
        private readonly string EnterSearchTerm = $"Enter {MinimumSearchTermLength} or more characters";       

        public ValidationResponse Validate(string searchTerm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < MinimumSearchTermLength)
                validationResponse.Errors.Add(new ValidationErrorDetail(SearchTerm, EnterSearchTerm));

            return validationResponse;
        }
    }

    public interface IRoatpSearchTermValidator
    {
        ValidationResponse Validate(string searchTerm);
    }
}
