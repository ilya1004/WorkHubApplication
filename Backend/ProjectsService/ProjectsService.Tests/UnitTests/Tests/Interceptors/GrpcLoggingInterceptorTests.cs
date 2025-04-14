using Grpc.Core;
using Grpc.Core.Testing;
using ProjectsService.API.Interceptors;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.Interceptors;

public class GrpcLoggingInterceptorTests
{
    private readonly Mock<ILogger<GrpcLoggingInterceptor>> _loggerMock;
    private readonly GrpcLoggingInterceptor _interceptor;
    private readonly ServerCallContext _context;
    private readonly Mock<UnaryServerMethod<object, object>> _continuationMock;

    public GrpcLoggingInterceptorTests()
    {
        _loggerMock = new Mock<ILogger<GrpcLoggingInterceptor>>();
        _interceptor = new GrpcLoggingInterceptor(_loggerMock.Object);
        _context = TestServerCallContext.Create(
            method: "/test/TestMethod",
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(5),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: null,
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => null,
            writeOptionsSetter: _ => { });
        _continuationMock = new Mock<UnaryServerMethod<object, object>>();
    }

    [Fact]
    public async Task UnaryServerHandler_SuccessfulCall_LogsInformationAndReturnsResponse()
    {
        // Arrange
        var request = new object();
        var expectedResponse = new object();
        _continuationMock.Setup(c => c.Invoke(request, _context))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _interceptor.UnaryServerHandler(request, _context, _continuationMock.Object);

        // Assert
        response.Should().Be(expectedResponse);
        _continuationMock.Verify(c => c.Invoke(request, _context), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            msg => msg.Contains("Completed gRPC call /test/TestMethod") &&
                   msg.Contains("ms with status OK"),
            Times.Once());
    }

    [Fact]
    public async Task UnaryServerHandler_FailedCall_LogsInformationWithErrorStatus()
    {
        // Arrange
        var request = new object();
        var exception = new RpcException(new Status(StatusCode.NotFound, "Not found"));
        _continuationMock.Setup(c => c.Invoke(request, _context))
            .ThrowsAsync(exception);

        // Act
        Func<Task> act = async () => await _interceptor.UnaryServerHandler(request, _context, _continuationMock.Object);

        // Assert
        await act.Should().ThrowAsync<RpcException>();
        _continuationMock.Verify(c => c.Invoke(request, _context), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "", Times.Never());
    }

    [Fact]
    public async Task UnaryServerHandler_SuccessfulCall_LogsElapsedTime()
    {
        // Arrange
        var request = new object();
        var expectedResponse = new object();
        _continuationMock.Setup(c => c.Invoke(request, _context))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _interceptor.UnaryServerHandler(request, _context, _continuationMock.Object);

        // Assert
        response.Should().Be(expectedResponse);
        _loggerMock.VerifyLog(LogLevel.Information, 
            msg => msg.Contains("Completed gRPC call /test/TestMethod") &&
                   msg.Contains("ms with status OK"),
            Times.Once());
    }
}