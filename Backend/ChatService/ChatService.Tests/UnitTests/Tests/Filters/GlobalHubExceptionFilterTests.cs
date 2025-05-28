using System.Security.Claims;
using ChatService.API.Filters;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Tests.UnitTests.Tests.Filters;

public class GlobalHubExceptionFilterTests
{
    private readonly Mock<ILogger<GlobalHubExceptionFilter>> _loggerMock;
    private readonly GlobalHubExceptionFilter _filter;
    private readonly HubInvocationContext _invocationContext;

    public GlobalHubExceptionFilterTests()
    {
        _loggerMock = new Mock<ILogger<GlobalHubExceptionFilter>>();
        _filter = new GlobalHubExceptionFilter(_loggerMock.Object);
        
        var clientsMock = new Mock<IHubCallerClients<IChatClient>>();
        var callerMock = new Mock<IChatClient>();
        clientsMock.Setup(c => c.Caller).Returns(callerMock.Object);
        
        var hubMock = new Mock<ChatHub>(
            new Mock<IMediator>().Object,
            new Mock<IMapper>().Object,
            new Mock<ILogger<ChatHub>>().Object)
        {
            CallBase = true
        };
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "user123") }));
        var contextMock = new Mock<HubCallerContext>();
        contextMock.Setup(c => c.User).Returns(user);
        var serviceProviderMock = new Mock<IServiceProvider>();
        
        var methodInfo = typeof(ChatHub).GetMethod("GetChatById") ?? throw new Exception("Method GetChatById not found");
        _invocationContext = new HubInvocationContext(
            contextMock.Object,
            serviceProviderMock.Object,
            hubMock.Object,
            methodInfo,
            new object[] { Guid.NewGuid() });
    }

    [Fact]
    public async Task InvokeMethodAsync_SuccessfulExecution_LogsInformationAndReturnsResult()
    {
        // Arrange
        var expectedResult = new object();
        async ValueTask<object?> Next(HubInvocationContext ctx)
        {
            return await Task.FromResult(expectedResult);
        }

        // Act
        var result = await _filter.InvokeMethodAsync(_invocationContext, Next);

        // Assert
        result.Should().Be(expectedResult);
        _loggerMock.VerifyLog(LogLevel.Information, Times.Once());
    }
}