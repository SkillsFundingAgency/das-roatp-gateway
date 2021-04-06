using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Domain;
using SFA.DAS.RoatpGateway.Web.Models;
using SFA.DAS.RoatpGateway.Web.Validators;
using SFA.DAS.RoatpGateway.Web.ViewModels;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Validators
{
    [TestFixture]
    public class GatewayValidatorTests
    {
        private RoatpGatewayPageViewModel _viewModel;

        private IRoatpGatewayPageValidator _validator;
        private const string ClarificationAnswer = "Clarification answer";

        [SetUp]
        public void Setup()
        {
            _validator = new RoatpGatewayPageValidator();
        }

        [TestCase(SectionReviewStatus.Pass, "", "", "", "", false)]
        [TestCase(SectionReviewStatus.Pass, "pass message goes here", "", "", "", false)]
        [TestCase(SectionReviewStatus.InProgress, "", "", "", "", false)]
        [TestCase(SectionReviewStatus.InProgress, "", "in progress message goes here", "", "", false)]
        [TestCase(SectionReviewStatus.Fail, "", "", "", "", true)]
        [TestCase(SectionReviewStatus.Fail, "", "", "fail message goes here", "", false)]
        [TestCase(SectionReviewStatus.Clarification, "", "", "", "", true)]
        [TestCase(SectionReviewStatus.Clarification, "", "", "", "clarification message goes here", false)]
        [TestCase(null, "", "", "", "", true)]
        public void test_cases_for_no_status_and_no_fail_text_to_check_messages_as_expected(string status, string passMessage, string inProgressMessage, string failMessage, string clarificationMessage, bool hasErrorMessage)
        {
            _viewModel = new RoatpGatewayPageViewModel
            {
                Status = status,
                OptionPassText = passMessage,
                OptionFailText = failMessage,
                OptionInProgressText = inProgressMessage,
                OptionClarificationText = clarificationMessage,
                PageId = GatewayPageIds.LegalName
            };

            var command = new SubmitGatewayPageAnswerCommand(_viewModel);

            var result = _validator.Validate(command).Result;

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());
        }

        [TestCase(150, false)]
        [TestCase(151, true)]
        public void test_cases_where_input_is_too_long(int wordCount, bool hasErrorMessage)
        {
            var words = string.Empty;
            for (var i = 0; i < wordCount; i++)
            {
                words = $"{words}{i} ";
            }

            _viewModel = new RoatpGatewayPageViewModel();
            _viewModel.Status = SectionReviewStatus.Pass;
            _viewModel.OptionPassText = words;

            var command = new SubmitGatewayPageAnswerCommand(_viewModel);

            var result = _validator.Validate(command).Result;

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());

        }




        [TestCase(SectionReviewStatus.Pass, "", "", "", "", ClarificationAnswer, false)]
        [TestCase(SectionReviewStatus.Pass, "pass message goes here", "", "", "", ClarificationAnswer, false)]
        [TestCase(SectionReviewStatus.InProgress, "", "", "", "", ClarificationAnswer, false)]
        [TestCase(SectionReviewStatus.InProgress, "", "in progress message goes here", "", "", ClarificationAnswer, false)]
        [TestCase(SectionReviewStatus.Fail, "", "", "fail message goes here", "", ClarificationAnswer, false)] 
        [TestCase(null, "", "", "", "", ClarificationAnswer, true)]
        [TestCase(SectionReviewStatus.Pass, "", "", "", "", null, true)]
        [TestCase(SectionReviewStatus.Pass, "pass message goes here", "", "", "", null, true)]
        [TestCase(SectionReviewStatus.InProgress, "", "", "", "", null, true)]
        [TestCase(SectionReviewStatus.InProgress, "", "in progress message goes here", "", "", null, true)]
        [TestCase(SectionReviewStatus.Fail, "", "", "", "", null, true)]
        [TestCase(SectionReviewStatus.Fail, "", "", "fail message goes here", "", null, true)]

        public void clarification_test_cases_for_no_status_and_no_fail_text_to_check_messages_as_expected(string status, string passMessage, string inProgressMessage, string failMessage, string clarificationMessage, string clarificationAnswer, bool hasErrorMessage)
        {
            _viewModel = new RoatpGatewayPageViewModel
            {
                Status = status,
                OptionPassText = passMessage,
                OptionFailText = failMessage,
                OptionInProgressText = inProgressMessage,
                OptionClarificationText = clarificationMessage,
                PageId = GatewayPageIds.LegalName,
                ClarificationAnswer = clarificationAnswer
            };

            var command = new SubmitGatewayPageAnswerCommand(_viewModel);

            var result = _validator.ValidateClarification(command).Result;

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());
        }

        [TestCase(typeof(RoatpGatewayPageViewModel))]
        [TestCase(typeof(CriminalCompliancePageViewModel))]
        public void clarification_test_cases_where_ClarificationAnswer_is_too_long(Type t)
        {
            _viewModel = (RoatpGatewayPageViewModel)Activator.CreateInstance(t);
            _viewModel.Status = SectionReviewStatus.Pass;
            _viewModel.OptionPassText = "words";
            _viewModel.ClarificationAnswer = string.Join(" ", Enumerable.Range(0, _viewModel.ClarificationAnswerMaxWords + 1));

            var command = new SubmitGatewayPageAnswerCommand(_viewModel);

            var result = _validator.ValidateClarification(command).Result;

            CollectionAssert.IsNotEmpty(result.Errors);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains($"{_viewModel.ClarificationAnswerMaxWords} words"));
        }
    }
}
