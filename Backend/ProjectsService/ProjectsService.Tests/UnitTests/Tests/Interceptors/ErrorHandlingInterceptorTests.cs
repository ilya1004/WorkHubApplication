using Grpc.Core;
using ProjectsService.API.Interceptors;
using ProjectsService.Application.Exceptions;
using Grpc.Core.Testing;

namespace ProjectsService.Tests.UnitTests.Tests.Interceptors;

public class ErrorHandlingInterceptorTests
{
    private readonly ErrorHandlingInterceptor _interceptor;
    private readonly ServerCallContext _context;
    private readonly Mock<UnaryServerMethod<object, object>> _continuationMock;

    public ErrorHandlingInterceptorTests()
    {
        _interceptor = new ErrorHandlingInterceptor();
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
    public async Task UnaryServerHandler_SuccessfulCall_ReturnsResponse()
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
    }

    [Theory]
    [InlineData(typeof(BadRequestException), StatusCode.InvalidArgument)]
    [InlineData(typeof(AlreadyExistsException), StatusCode.AlreadyExists)]
    [InlineData(typeof(NotFoundException), StatusCode.NotFound)]
    [InlineData(typeof(UnauthorizedException), StatusCode.Unauthenticated)]
    [InlineData(typeof(ForbiddenException), StatusCode.PermissionDenied)]
    [InlineData(typeof(Exception), StatusCode.Internal)]
    public async Task UnaryServerHandler_ExceptionThrown_ThrowsRpcExceptionWithCorrectStatus(Type exceptionType, StatusCode expectedStatusCode)
    {
        // Arrange
        var request = new object();
        var exceptionMessage = "Test error";
        var exception = Activator.CreateInstance(exceptionType, exceptionMessage) as Exception;
        _continuationMock.Setup(c => c.Invoke(request, _context))
            .ThrowsAsync(exception!);

        // Act
        Func<Task> act = async () => await _interceptor.UnaryServerHandler(request, _context, _continuationMock.Object);

        // Assert
        var rpcException = await act.Should().ThrowAsync<RpcException>();
        rpcException.Which.Status.StatusCode.Should().Be(expectedStatusCode);
        rpcException.Which.Status.Detail.Should().Be(exceptionMessage);
        _continuationMock.Verify(c => c.Invoke(request, _context), Times.Once());
    }

    [Fact]
    public async Task UnaryServerHandler_ExceptionWithInnerException_ThrowsRpcExceptionWithInnerException()
    {
        // Arrange
        var request = new object();
        var innerException = new InvalidOperationException("Inner error");
        var exception = new Exception("Outer error", innerException);
        _continuationMock.Setup(c => c.Invoke(request, _context))
            .ThrowsAsync(exception);

        // Act
        Func<Task> act = async () => await _interceptor.UnaryServerHandler(request, _context, _continuationMock.Object);

        // Assert
        var rpcException = await act.Should().ThrowAsync<RpcException>();
        rpcException.Which.Status.StatusCode.Should().Be(StatusCode.Internal);
        rpcException.Which.Status.Detail.Should().Be("Outer error");
        _continuationMock.Verify(c => c.Invoke(request, _context), Times.Once());
    }
}