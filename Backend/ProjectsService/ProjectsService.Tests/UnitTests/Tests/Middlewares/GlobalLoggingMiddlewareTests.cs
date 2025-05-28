using Microsoft.AspNetCore.Http;
using ProjectsService.API.Middlewares;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.Middlewares;

public class GlobalLoggingMiddlewareTests
{
    private readonly Mock<ILogger<GlobalLoggingMiddleware>> _loggerMock;
    private readonly GlobalLoggingMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public GlobalLoggingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalLoggingMiddleware>>();
        _middleware = new GlobalLoggingMiddleware(_loggerMock.Object);
        _httpContext = new DefaultHttpContext
        {
            Request =
            {
                Method = "GET",
                Path = "/api/test",
                QueryString = new QueryString("?param=value")
            },
            Response =
            {
                StatusCode = StatusCodes.Status200OK
            }
        };
    }

    [Fact]
    public async Task InvokeAsync_SuccessfulRequest_LogsRequestInformation()
    {
        // Arrange
        RequestDelegate next = ctx => Task.CompletedTask;

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _loggerMock.VerifyLog(LogLevel.Information,
            msg => msg.Contains("HTTP GET /api/test?param=value => 200 in") &&
                   msg.Contains("ms"),
            Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_RequestWithDifferentStatusCode_LogsCorrectStatusCode()
    {
        // Arrange
        _httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        RequestDelegate next = ctx => Task.CompletedTask;

        // Act
        await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        _loggerMock.VerifyLog(LogLevel.Information,
            msg => msg.Contains("HTTP GET /api/test?param=value => 404 in") &&
                   msg.Contains("ms"),
            Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_RequestWithException_LogsAfterException()
    {
        // Arrange
        RequestDelegate next = ctx => throw new Exception("Test exception");
        _httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Act
        var act = async () => await _middleware.InvokeAsync(_httpContext, next);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        _loggerMock.VerifyLog(LogLevel.Information, "", Times.Never());
    }
}