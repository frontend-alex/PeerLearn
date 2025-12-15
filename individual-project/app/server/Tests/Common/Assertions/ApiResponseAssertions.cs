using API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace PeerLearn.Tests.Common.Assertions;

/// <summary>
/// Custom assertions for API responses
/// </summary>
public static class ApiResponseAssertions
{
    public static void ShouldBeSuccessfulApiResponse<T>(this IActionResult result)
    {
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<ApiResponse<T>>().Subject;
        response.Success.Should().BeTrue();
    }

    public static ApiResponse<T> ShouldBeSuccessfulApiResponse<T>(this IActionResult result, string? expectedMessage = null)
    {
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<ApiResponse<T>>().Subject;
        response.Success.Should().BeTrue();
        
        if (expectedMessage != null)
        {
            response.Message.Should().Be(expectedMessage);
        }
        
        return response;
    }

    public static void ShouldBeErrorResponse(this IActionResult result, int expectedStatusCode)
    {
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(expectedStatusCode);
    }

    public static void ShouldBeBadRequest(this IActionResult result)
    {
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    public static void ShouldBeUnauthorized(this IActionResult result)
    {
        result.Should().BeOfType<UnauthorizedResult>();
    }
}

