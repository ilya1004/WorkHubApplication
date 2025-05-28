using ChatService.API.Filters;
using ChatService.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Tests.UnitTests.Tests.Filters;

public class GlobalHubLoggingFilterTests
{
    private readonly Mock<ILogger<GlobalHubLoggingFilter>> _loggerMock;
    private readonly GlobalHubLoggingFilter _filter;
    private readonly HubInvocationContext _invocationContext;

    public GlobalHubLoggingFilterTests()
    {
        _loggerMock = new Mock<ILogger<GlobalHubLoggingFilter>>();
        _filter = new GlobalHubLoggingFilter(_loggerMock.Object);
        var contextMock = new Mock<HubCallerContext>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        
        var hub = new ChatHub(
            new Mock<IMediator>().Object,
            new Mock<IMapper>().Object,
            new Mock<ILogger<ChatHub>>().Object);
        
        var methodInfo = typeof(ChatHub).GetMethod("GetChatById") ?? throw new Exception("Method GetChatById not found");
        _invocationContext = new HubInvocationContext(
            contextMock.Object,
            serviceProviderMock.Object,
            hub,
            methodInfo,
            new object[] { Guid.NewGuid() });
    }

    [Fact]
    public async Task InvokeMethodAsync_ExecutesMethod_LogsDuration()
    {
        // Arrange
        var expectedResult = new object();
        async ValueTask<object?> Next(HubInvocationContext ctx)
        {
            await Task.Delay(50);
            return expectedResult;
        }

        // Act
        var result = await _filter.InvokeMethodAsync(_invocationContext, Next);

        // Assert
        result.Should().Be(expectedResult);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SignalR ChatHub.GetChatById executed in") && v.ToString()!.Contains("ms")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once());
    }
}