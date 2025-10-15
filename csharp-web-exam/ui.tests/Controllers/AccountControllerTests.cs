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
        private Mock<UrlHelper> _mockUrlHelper;

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

            // Setup cookies
            var requestCookies = new HttpCookieCollection();
            var responseCookies = new HttpCookieCollection();
            
            _mockRequest.Setup(r => r.Cookies).Returns(requestCookies);
            _mockResponse.Setup(r => r.Cookies).Returns(responseCookies);
            _mockRequest.Setup(r => r.Url).Returns(new System.Uri("http://localhost/Account/Login"));
            
            _mockHttpContext.Setup(ctx => ctx.Request).Returns(_mockRequest.Object);
            _mockHttpContext.Setup(ctx => ctx.Response).Returns(_mockResponse.Object);
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);

            // Setup UrlHelper
            var routeData = new RouteData();
            var requestContext = new RequestContext(_mockHttpContext.Object, routeData);
            _controller.ControllerContext = new ControllerContext(requestContext, _controller);
            
            _mockUrlHelper = new Mock<UrlHelper>();
            _mockUrlHelper.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);
            _controller.Url = _mockUrlHelper.Object;
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

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }
    }
}
