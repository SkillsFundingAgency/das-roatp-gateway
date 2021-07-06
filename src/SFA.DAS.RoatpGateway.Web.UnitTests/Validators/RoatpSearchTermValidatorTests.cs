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
            var searchTerm = "";
            var response = _validator.Validate(searchTerm);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual($"Enter {MinimumLength} or more characters", response.Errors.First().ErrorMessage);
            Assert.AreEqual("SearchTerm", response.Errors.First().Field);
        }

        [Test]
        public void When_searchTerm_is_less_than_minimum_length_then_an_error_is_returned()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength - 1));
            var response = _validator.Validate(searchTerm);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual($"Enter {MinimumLength} or more characters", response.Errors.First().ErrorMessage);
            Assert.AreEqual("SearchTerm", response.Errors.First().Field);
        }

        [Test]
        public void When_searchTerm_is_minimum_length_then_validation_passes()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength));
            var response = _validator.Validate(searchTerm);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public void When_searchTerm_is_greater_than_minimum_length_then_validation_passes()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength + 1));
            var response = _validator.Validate(searchTerm);

            Assert.IsTrue(response.IsValid);
        }
    }
}
