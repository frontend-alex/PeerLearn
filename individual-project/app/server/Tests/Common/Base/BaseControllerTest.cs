using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace PeerLearn.Tests.Common.Base;

/// <summary>
/// Base class for controller unit tests providing common setup
/// </summary>
public abstract class BaseControllerTest
{
    protected const int TestUserId = 1;
    protected const int TestWorkspaceId = 1;
    protected const int TestDocumentId = 1;

    /// <summary>
    /// Creates a mock HttpContext with authenticated user
    /// </summary>
    protected HttpContext CreateAuthenticatedHttpContext(int? userId = null)
    {
        var userIdValue = userId ?? TestUserId;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userIdValue.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        return httpContext;
    }

    /// <summary>
    /// Sets up controller context with authenticated user
    /// </summary>
    protected ControllerContext CreateAuthenticatedControllerContext(int? userId = null)
    {
        return new ControllerContext
        {
            HttpContext = CreateAuthenticatedHttpContext(userId)
        };
    }

    /// <summary>
    /// Creates a mock HttpContext with response cookies
    /// </summary>
    protected (HttpContext HttpContext, Mock<IResponseCookies> MockCookies) CreateHttpContextWithCookies()
    {
        var mockHttpContext = new Mock<HttpContext>();
        var mockResponse = new Mock<HttpResponse>();
        var mockCookies = new Mock<IResponseCookies>();

        mockResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);

        return (mockHttpContext.Object, mockCookies);
    }
}

