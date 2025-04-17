using System.Security.Claims;
using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatByProjectId;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Tests.UnitTests.Tests.Hubs;

public class ChatHubTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<ChatHub>> _loggerMock = new();
    private readonly Mock<IHubCallerClients<IChatClient>> _clientsMock = new();
    private readonly Mock<IChatClient> _callerClientMock = new();
    private readonly Mock<IChatClient> _userClientMock = new();
    private readonly Mock<HubCallerContext> _hubCallerContextMock = new();
    private readonly ChatHub _hub;

    public ChatHubTests()
    {
        _hub = new ChatHub(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object);
        
        _hubCallerContextMock.Setup(c => c.ConnectionId).Returns("conn123");
        _hubCallerContextMock.Setup(c => c.UserIdentifier).Returns("user123");
        _hubCallerContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, "user123")
        ])));
        
        _clientsMock.Setup(c => c.Caller).Returns(_callerClientMock.Object);
        _clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_userClientMock.Object);
        
        _hub.Context = _hubCallerContextMock.Object;
        _hub.Clients = _clientsMock.Object;
    }

    [Fact]
    public async Task OnConnectedAsync_LogsUserConnection()
    {
        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _loggerMock.VerifyLog(LogLevel.Information, "User user123 connected with connection ID conn123", Times.Once());
    }

    [Fact]
    public async Task OnDisconnectedAsync_WithoutException_LogsDisconnection()
    {
        // Act
        await _hub.OnDisconnectedAsync(null);

        // Assert
        _loggerMock.VerifyLog(LogLevel.Information, "User user123 disconnected (Connection ID: conn123)", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Error, It.IsAny<string>(), Times.Never());
    }

    [Fact]
    public async Task OnDisconnectedAsync_WithException_LogsDisconnectionAndError()
    {
        // Arrange
        var exception = new Exception("Disconnect error");

        // Act
        await _hub.OnDisconnectedAsync(exception);

        // Assert
        _loggerMock.VerifyLog(LogLevel.Information, "User user123 disconnected (Connection ID: conn123)", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Error, "Disconnection error for user user123", Times.Once());
    }

    [Fact]
    public async Task CreateChat_SendsCommandAndLogs()
    {
        // Arrange
        var request = new CreateChatRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var command = new CreateChatCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _mapperMock.Setup(m => m.Map<CreateChatCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _hub.CreateChat(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Chat created successfully", Times.Once());
    }

    [Fact]
    public async Task GetChatById_SendsQueryAndNotifiesCaller()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var query = new GetChatByIdQuery(chatId);
        var chat = new Chat { Id = chatId };
        _mapperMock.Setup(m => m.Map<GetChatByIdQuery>(chatId)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

        // Act
        await _hub.GetChatById(chatId);

        // Assert
        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once());
        _callerClientMock.Verify(c => c.ReceiveChat(chat), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Chat retrieved successfully", Times.Once());
    }

    [Fact]
    public async Task GetChatByProjectId_SendsQueryAndNotifiesCaller()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetChatByProjectIdQuery(projectId);
        var chat = new Chat { Id = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<GetChatByProjectIdQuery>(projectId)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

        // Act
        await _hub.GetChatByProjectId(projectId);

        // Assert
        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once());
        _callerClientMock.Verify(c => c.ReceiveChat(chat), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Chat retrieved successfully", Times.Once());
    }

    [Fact]
    public async Task SetChatInactive_SendsCommandAndLogs()
    {
        // Arrange
        var request = new SetChatInactiveRequest(Guid.NewGuid());
        var command = new SetChatInactiveCommand(Guid.NewGuid());
        _mapperMock.Setup(m => m.Map<SetChatInactiveCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _hub.SetChatInactive(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Chat by project with ID '{request.ProjectId}' set inactive", Times.Once());
    }

    [Fact]
    public async Task SendTextMessage_SendsCommandAndNotifiesClients()
    {
        // Arrange
        var request = new CreateTextMessageRequest(Guid.NewGuid(), Guid.NewGuid(), "Hello");
        var command = new CreateTextMessageCommand(request.ChatId, request.ReceiverId, request.Text);
        var message = new Message { Id = Guid.NewGuid(), Text = "Hello" };
        _mapperMock.Setup(m => m.Map<CreateTextMessageCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(message);

        // Act
        await _hub.SendTextMessage(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once());
        _callerClientMock.Verify(c => c.ReceiveTextMessage(message), Times.Once());
        _userClientMock.Verify(c => c.ReceiveTextMessage(message), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Text message sent successfully", Times.Once());
    }

    [Fact]
    public async Task GetChatMessages_SendsQueryAndNotifiesCaller()
    {
        // Arrange
        var request = new GetChatMessagesRequest(Guid.NewGuid(), 1, 10);
        var query = new GetChatMessagesQuery(request.ChatId, request.PageNo, request.PageSize);
        var result = new PaginatedResultModel<Message>
        {
            Items = [new Message { Id = Guid.NewGuid() }],
            TotalCount = 1
        };
        _mapperMock.Setup(m => m.Map<GetChatMessagesQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        await _hub.GetChatMessages(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once());
        _callerClientMock.Verify(c => c.ReceiveChatMessages(result), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Retrieved 1 messages", Times.Once());
    }

    [Fact]
    public async Task DeleteMessage_SendsCommandAndNotifiesClients()
    {
        // Arrange
        var request = new DeleteMessageRequest(Guid.NewGuid(), Guid.NewGuid());
        var command = new DeleteMessageCommand(request.MessageId);
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _hub.DeleteMessage(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once());
        _callerClientMock.Verify(c => c.MessageIsDeleted(request.MessageId), Times.Once());
        _userClientMock.Verify(c => c.MessageIsDeleted(request.MessageId), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Message with ID '{request.MessageId}' deleted successfully", Times.Once());
    }
}