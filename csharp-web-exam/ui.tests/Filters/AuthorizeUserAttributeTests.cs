using csharp_web_exam.Filters;
using Moq;
using System.Web;
using System.Web.Mvc;

namespace ui.tests.Filters
{
    /// <summary>
    /// Testable version of AuthorizeUserAttribute that exposes protected methods
    /// </summary>
    public class TestableAuthorizeUserAttribute : AuthorizeUserAttribute
    {
        public bool TestAuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        public void TestHandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
    }

    [TestClass]
    public class AuthorizeUserAttributeTests
    {
        private TestableAuthorizeUserAttribute _filter;
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpRequestBase> _mockRequest;
        private Mock<HttpSessionStateBase> _mockSession;

        [TestInitialize]
        public void Setup()
        {
            _filter = new TestableAuthorizeUserAttribute();

            // Setup mock HTTP context
            _mockHttpContext = new Mock<HttpContextBase>();
            _mockRequest = new Mock<HttpRequestBase>();
            _mockSession = new Mock<HttpSessionStateBase>();

            // Setup request properties
            _mockRequest.Setup(r => r.Url).Returns(new System.Uri("http://localhost/Products"));
            _mockRequest.Setup(r => r.RawUrl).Returns("/Products");
            
            // Setup context
            _mockHttpContext.Setup(ctx => ctx.Request).Returns(_mockRequest.Object);
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);
        }

        [TestMethod]
        public void AuthorizeCore_WithValidTokenAndSession_ReturnsTrue()
        {
            // Arrange
            HttpCookieCollection cookies = [new HttpCookie("AuthToken", "valid-token")];
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            bool isAuthorized = _filter.TestAuthorizeCore(_mockHttpContext.Object);

            // Assert
            Assert.IsTrue(isAuthorized, "User with valid token and session should be authorized");
        }

        [TestMethod]
        public void AuthorizeCore_WithoutToken_ReturnsFalse()
        {
            // Arrange
            _mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());
            _mockSession.Setup(s => s["Username"]).Returns(null);

            // Act
            bool isAuthorized = _filter.TestAuthorizeCore(_mockHttpContext.Object);

            // Assert
            Assert.IsFalse(isAuthorized, "User without token should not be authorized");
        }

        [TestMethod]
        public void AuthorizeCore_WithTokenButNoSession_ReturnsFalse()
        {
            // Arrange
            HttpCookieCollection cookies =
            [
                new HttpCookie("AuthToken", "valid-token")
            ];
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns(null);

            // Act
            bool isAuthorized = _filter.TestAuthorizeCore(_mockHttpContext.Object);

            // Assert
            Assert.IsFalse(isAuthorized, "User with token but no session should not be authorized");
        }

        [TestMethod]
        public void AuthorizeCore_WithSessionButNoToken_ReturnsFalse()
        {
            // Arrange
            _mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            bool isAuthorized = _filter.TestAuthorizeCore(_mockHttpContext.Object);

            // Assert
            Assert.IsFalse(isAuthorized, "User with session but no token should not be authorized");
        }

        [TestMethod]
        public void AuthorizeCore_WithEmptyToken_ReturnsFalse()
        {
            // Arrange
            HttpCookieCollection cookies =
            [
                new HttpCookie("AuthToken", "")
            ];
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            bool isAuthorized = _filter.TestAuthorizeCore(_mockHttpContext.Object);

            // Assert
            Assert.IsFalse(isAuthorized, "User with empty token should not be authorized");
        }

        [TestMethod]
        public void AuthorizeCore_WithEmptyUsername_ReturnsFalse()
        {
            // Arrange
            HttpCookieCollection cookies =
            [
                new HttpCookie("AuthToken", "valid-token")
            ];
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns("");

            // Act
            bool isAuthorized = _filter.TestAuthorizeCore(_mockHttpContext.Object);

            // Assert
            Assert.IsFalse(isAuthorized, "User with empty username should not be authorized");
        }

        [TestMethod]
        public void AuthorizeCore_WithNullHttpContext_ThrowsArgumentNullException()
        {
            // Arrange
            HttpContextBase nullContext = null;

            // Act & Assert
            Assert.ThrowsException<System.ArgumentNullException>(() =>
            {
                _filter.TestAuthorizeCore(nullContext);
            }, "Should throw ArgumentNullException when HttpContext is null");
        }

        [TestMethod]
        public void HandleUnauthorizedRequest_RedirectsToLogin()
        {
            // Arrange
            Mock<ControllerBase> mockController = new();
            System.Web.Routing.RouteData routeData = new();
            routeData.Values.Add("controller", "Products");
            routeData.Values.Add("action", "Index");

            ControllerContext controllerContext = new(
                _mockHttpContext.Object,
                routeData,
                mockController.Object
            );

            Mock<ActionDescriptor> mockActionDescriptor = new();
            AuthorizationContext authContext = new(controllerContext, mockActionDescriptor.Object);

            // Act
            _filter.TestHandleUnauthorizedRequest(authContext);

            // Assert
            Assert.IsNotNull(authContext.Result, "Result should be set");
            Assert.IsInstanceOfType(authContext.Result, typeof(RedirectToRouteResult), "Should redirect to route");

            RedirectToRouteResult redirectResult = authContext.Result as RedirectToRouteResult;
            Assert.AreEqual("Account", redirectResult.RouteValues["controller"], "Should redirect to Account controller");
            Assert.AreEqual("Login", redirectResult.RouteValues["action"], "Should redirect to Login action");
            Assert.IsNotNull(redirectResult.RouteValues["returnUrl"], "Should include returnUrl");
        }
    }
}
