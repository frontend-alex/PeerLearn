using FluentAssertions;
using System.Net;

namespace PeerLearn.Tests.Common.Assertions;

/// <summary>
/// Custom assertions for HTTP responses
/// </summary>
public static class HttpResponseAssertions
{
    public static void ShouldBeOk(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public static void ShouldBeBadRequest(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static void ShouldBeUnauthorized(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public static void ShouldBeNotFound(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static void ShouldBeForbidden(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public static void ShouldBeConflict(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public static void ShouldBeOneOf(this HttpResponseMessage response, params HttpStatusCode[] statusCodes)
    {
        response.StatusCode.Should().BeOneOf(statusCodes);
    }
}

