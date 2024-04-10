using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Web.Validators;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Validators
{
    [TestFixture]
    public class RoatpSearchTermValidatorTests
    {
        private const int MinimumLength = 3;
        private readonly RoatpSearchTermValidator _validator = new RoatpSearchTermValidator();

        [Test]
        public void When_searchTerm_is_not_provided_then_an_error_is_returned()
        {
            var response = _validator.Validate(null);

            Assert.That(response.IsValid, Is.False);
            Assert.That($"Enter an organisation name or UKPRN", Is.EqualTo(response.Errors.First().ErrorMessage));
            Assert.That("SearchTerm", Is.EqualTo(response.Errors.First().Field));
        }

        [Test]
        public void When_searchTerm_is_empty_string_then_an_error_is_returned()
        {
            var searchTerm = string.Empty;
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.False);
            Assert.That($"Enter an organisation name or UKPRN", Is.EqualTo(response.Errors.First().ErrorMessage));
            Assert.That("SearchTerm", Is.EqualTo(response.Errors.First().Field));
        }

        [Test]
        public void When_searchTerm_is_whitespace_only_then_an_error_is_returned()
        {
            var searchTerm = string.Concat(Enumerable.Repeat(" ", MinimumLength)); ;
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.False);
            Assert.That($"Enter an organisation name or UKPRN", Is.EqualTo(response.Errors.First().ErrorMessage));
            Assert.That("SearchTerm", Is.EqualTo(response.Errors.First().Field));
        }

        [Test]
        public void When_searchTerm_is_less_than_minimum_length_then_an_error_is_returned()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength - 1));
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.False);
            Assert.That($"Enter a UKPRN or an organisation name using {MinimumLength} or more characters", Is.EqualTo(response.Errors.First().ErrorMessage));
            Assert.That("SearchTerm", Is.EqualTo(response.Errors.First().Field));
        }

        [Test]
        public void When_searchTerm_is_minimum_length_then_validation_passes()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength));
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.True);
        }

        [Test]
        public void When_searchTerm_is_greater_than_minimum_length_then_validation_passes()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength + 1));
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.True);
        }
    }
}
