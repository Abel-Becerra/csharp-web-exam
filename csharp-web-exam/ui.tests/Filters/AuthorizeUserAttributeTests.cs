using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using csharp_web_exam.Filters;

namespace ui.tests.Filters
{
    [TestClass]
    public class AuthorizeUserAttributeTests
    {
        private AuthorizeUserAttribute _filter;
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<HttpRequestBase> _mockRequest;
        private Mock<HttpSessionStateBase> _mockSession;
        private AuthorizationContext _authContext;

        [TestInitialize]
        public void Setup()
        {
            _filter = new AuthorizeUserAttribute();

            // Setup mock HTTP context
            _mockHttpContext = new Mock<HttpContextBase>();
            _mockRequest = new Mock<HttpRequestBase>();
            _mockSession = new Mock<HttpSessionStateBase>();

            _mockHttpContext.Setup(ctx => ctx.Request).Returns(_mockRequest.Object);
            _mockHttpContext.Setup(ctx => ctx.Session).Returns(_mockSession.Object);
            _mockRequest.Setup(r => r.Url).Returns(new System.Uri("http://localhost/Products"));

            // Setup authorization context
            var controllerContext = new ControllerContext(
                _mockHttpContext.Object,
                new RouteData(),
                new Mock<ControllerBase>().Object
            );

            _authContext = new AuthorizationContext(controllerContext, new Mock<ActionDescriptor>().Object);
        }

        [TestMethod]
        public void OnAuthorization_WithValidTokenAndSession_AllowsAccess()
        {
            // Arrange
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("AuthToken", "valid-token"));
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            _filter.OnAuthorization(_authContext);

            // Assert
            Assert.IsNull(_authContext.Result); // No redirect means authorized
        }

        [TestMethod]
        public void OnAuthorization_WithoutToken_RedirectsToLogin()
        {
            // Arrange
            _mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());
            _mockSession.Setup(s => s["Username"]).Returns(null);

            // Act
            _filter.OnAuthorization(_authContext);

            // Assert
            Assert.IsNotNull(_authContext.Result);
            Assert.IsInstanceOfType(_authContext.Result, typeof(RedirectToRouteResult));
            var redirectResult = _authContext.Result as RedirectToRouteResult;
            Assert.AreEqual("Login", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Account", redirectResult.RouteValues["controller"]);
        }

        [TestMethod]
        public void OnAuthorization_WithTokenButNoSession_RedirectsToLogin()
        {
            // Arrange
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("AuthToken", "valid-token"));
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns(null);

            // Act
            _filter.OnAuthorization(_authContext);

            // Assert
            Assert.IsNotNull(_authContext.Result);
            Assert.IsInstanceOfType(_authContext.Result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void OnAuthorization_WithSessionButNoToken_RedirectsToLogin()
        {
            // Arrange
            _mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            _filter.OnAuthorization(_authContext);

            // Assert
            Assert.IsNotNull(_authContext.Result);
            Assert.IsInstanceOfType(_authContext.Result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void OnAuthorization_WithEmptyToken_RedirectsToLogin()
        {
            // Arrange
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("AuthToken", ""));
            _mockRequest.Setup(r => r.Cookies).Returns(cookies);
            _mockSession.Setup(s => s["Username"]).Returns("testuser");

            // Act
            _filter.OnAuthorization(_authContext);

            // Assert
            Assert.IsNotNull(_authContext.Result);
            Assert.IsInstanceOfType(_authContext.Result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void OnAuthorization_RedirectIncludesReturnUrl()
        {
            // Arrange
            _mockRequest.Setup(r => r.Cookies).Returns(new HttpCookieCollection());
            _mockSession.Setup(s => s["Username"]).Returns(null);
            _mockRequest.Setup(r => r.Url).Returns(new System.Uri("http://localhost/Products/Details/1"));

            // Act
            _filter.OnAuthorization(_authContext);

            // Assert
            Assert.IsNotNull(_authContext.Result);
            var redirectResult = _authContext.Result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult.RouteValues["returnUrl"]);
        }
    }
}
