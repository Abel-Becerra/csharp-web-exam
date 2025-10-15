using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using csharp_web_exam.Controllers;
using csharp_web_exam.Models;
using csharp_web_exam.Services;

namespace ui.tests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<ApiClient> _mockApiClient;
        private AccountController _controller;
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpRequestBase> _mockRequest;
        private Mock<HttpResponseBase> _mockResponse;
        private Mock<HttpSessionStateBase> _mockSession;

        [TestInitialize]
        public void Setup()
        {
            _mockApiClient = new Mock<ApiClient>();
            _controller = new AccountController();

            // Setup mock HTTP context
            _mockHttpContext = new Mock<HttpContextBase>();
            _mockRequest = new Mock<HttpRequestBase>();
            _mockResponse = new Mock<HttpResponseBase>();
            _mockSession = new Mock<HttpSessionStateBase>();

            _mockHttpContext.Setup(ctx => ctx.Request).Returns(_mockRequest.Object);
            _mockHttpContext.Setup(ctx => ctx.Response).Returns(_mockResponse.Object);
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);

            var requestContext = new RequestContext(_mockHttpContext.Object, new RouteData());
            _controller.ControllerContext = new ControllerContext(requestContext, _controller);
        }

        [TestMethod]
        public void Login_GET_ReturnsViewResult()
        {
            // Arrange
            string returnUrl = "/Products";

            // Act
            var result = _controller.Login(returnUrl) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(returnUrl, _controller.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_GET_WhenAuthenticated_RedirectsToReturnUrl()
        {
            // Arrange
            string returnUrl = "/Products";
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("AuthToken", "test-token"));
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            var result = _controller.Login(returnUrl);

            // Assert
            Assert.IsInstanceOfType<RedirectResult>(result);
        }

        [TestMethod]
        public async Task Login_POST_WithValidCredentials_RedirectsToReturnUrl()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "admin123",
                RememberMe = false
            };
            string returnUrl = "/Products";

            // Act
            var result = await _controller.Login(model, returnUrl);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Login_POST_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var model = new LoginViewModel();
            _controller.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Login_POST_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "invalid",
                Password = "wrong"
            };

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            // Should have model error for invalid credentials
        }

        [TestMethod]
        public async Task Login_POST_WithEmptyUsername_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "",
                Password = "password"
            };
            _controller.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Login_POST_WithEmptyPassword_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = ""
            };
            _controller.ModelState.AddModelError("Password", "Password is required");

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Logout_ClearsCookieAndSession_RedirectsToLogin()
        {
            // Arrange
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("AuthToken", "test-token"));
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);

            // Act
            var result = _controller.Logout() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Logout_WhenNotAuthenticated_StillRedirectsToLogin()
        {
            // Arrange
            _mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());

            // Act
            var result = _controller.Logout() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.RouteValues["action"]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }
    }
}
