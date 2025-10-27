using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.RoatpGateway.Web.Controllers;

namespace SFA.DAS.RoatpGateway.Web.UnitTests.Controllers.ErrorPage
{
    [TestFixture]
    public class ErrorPageControllerTests
    {
        private ErrorPageController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new ErrorPageController()
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public void PageNotFound_returns_PageNotFound_view()
        {
            var expectedView = "~/Views/ErrorPage/PageNotFound.cshtml";

            var result = _controller.PageNotFound() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(expectedView));
        }

        [Test]
        public void ServiceError_returns_ServiceError_view()
        {
            var expectedView = "~/Views/ErrorPage/ServiceError.cshtml";

            var result = _controller.ServiceError() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(expectedView));
        }

        [Test]
        public void ServiceUnavailable_returns_ServiceUnavailable_view()
        {
            var expectedView = "~/Views/ErrorPage/ServiceUnavailable.cshtml";

            var result = _controller.ServiceUnavailable() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(expectedView));
        }

        [Test]
        public void ServiceErrorHandler_redirects_to_ServiceError_action()
        {
            var expectedAction = "ServiceError";

            var result = _controller.ServiceErrorHandler() as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(expectedAction));
        }

        [Test]
        public void ServiceUnavailableHandler_redirects_to_ServiceUnavailable_action()
        {
            var expectedAction = "ServiceUnavailable";

            var result = _controller.ServiceUnavailableHandler() as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(expectedAction));
        }
    }
}
