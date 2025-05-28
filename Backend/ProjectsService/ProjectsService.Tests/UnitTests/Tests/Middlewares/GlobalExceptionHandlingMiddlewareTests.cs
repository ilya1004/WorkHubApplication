using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsService.API.Middlewares;
using ProjectsService.Application.Exceptions;

namespace ProjectsService.Tests.UnitTests.Tests.Middlewares;

public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly GlobalExceptionHandlingMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;
    private readonly MemoryStream _responseBodyStream;

    public GlobalExceptionHandlingMiddlewareTests()
    {
        _middleware = new GlobalExceptionHandlingMiddleware();
        _responseBodyStream = new MemoryStream();
        _httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = _responseBodyStream
            },
            Request =
            {
                Path = "/test"
            }
        };
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextDelegate()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        nextCalled.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Theory]
    [InlineData(typeof(BadRequestException), StatusCodes.Status400BadRequest, "BadRequestException", "Invalid request")]
    [InlineData(typeof(AlreadyExistsException), StatusCodes.Status400BadRequest, "AlreadyExistsException", "Already exists")]
    [InlineData(typeof(UnauthorizedException), StatusCodes.Status401Unauthorized, "UnauthorizedException", "Unauthorized access")]
    [InlineData(typeof(ForbiddenException), StatusCodes.Status403Forbidden, "ForbiddenException", "Forbidden access")]
    [InlineData(typeof(NotFoundException), StatusCodes.Status404NotFound, "NotFoundException", "Not found")]
    [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError, "Exception", "Unexpected error")]
    public async Task InvokeAsync_ExceptionThrown_ReturnsCorrectStatusCodeAndProblemDetails(
        Type exceptionType, int expectedStatusCode, string expectedType, string expectedMessage)
    {
        // Arrange
        var exception = Activator.CreateInstance(exceptionType, expectedMessage) as Exception;
        RequestDelegate next = _ => throw exception!;

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
        _httpContext.Response.ContentType.Should().Be("application/json");

        _responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_responseBodyStream).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody);

        problemDetails.Should().NotBeNull();
        problemDetails.Status.Should().Be(expectedStatusCode);
        problemDetails.Type.Should().Be(expectedType);
        problemDetails.Detail.Should().Be(expectedMessage);
        problemDetails.Instance.Should().Be(_httpContext.Request.Path);
        problemDetails.Title.Should().Be(expectedStatusCode == StatusCodes.Status500InternalServerError ? "Internal Server Error" : "Error");
    }
}