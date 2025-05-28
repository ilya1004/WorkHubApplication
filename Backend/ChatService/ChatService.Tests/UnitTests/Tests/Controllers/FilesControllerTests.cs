using ChatService.API.Contracts.ChatContracts;
using ChatService.API.Controllers;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Tests.UnitTests.Tests.Controllers;

public class FilesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<FilesController>> _loggerMock;
    private readonly FilesController _controller;
    private readonly Mock<IChatClient> _chatClientMock;

    public FilesControllerTests()
    {
        var hubContextMock = new Mock<IHubContext<ChatHub, IChatClient>>();
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<FilesController>>();
        _chatClientMock = new Mock<IChatClient>();

        _controller = new FilesController(
            hubContextMock.Object,
            _mediatorMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
        
        var clientsMock = new Mock<IHubClients<IChatClient>>();
        clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_chatClientMock.Object);
        hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
    }

    [Fact]
    public async Task UploadFile_ShouldReturnCreated_AndSendFileMessage()
    {
        // Arrange
        var request = new CreateFileMessageRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            It.IsAny<IFormFile>());
        var command = new CreateFileMessageCommand(
            request.ChatId,
            request.ReceiverId,
            new MemoryStream(),
            "application/pdf");
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            ReceiverId = request.ReceiverId,
            SenderId = Guid.NewGuid(),
            Type = MessageType.File
        };

        _mapperMock.Setup(m => m.Map<CreateFileMessageCommand>(request)).Returns(command);
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(message);
        _chatClientMock.Setup(c => c.ReceiveFileMessage(It.IsAny<Message>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UploadFile(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once());
        _chatClientMock.Verify(
            c => c.ReceiveFileMessage(message),
            Times.Exactly(2));
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"File uploaded for receiver with ID '{request.ReceiverId}'", Times.Once());
    }

    [Fact]
    public async Task GetMessageFileById_ShouldReturnFileResult()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var fileStream = new MemoryStream();
        var contentType = "application/pdf";
        var fileResponse = new FileResponse(fileStream, contentType);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetMessageFileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileResponse);

        // Act
        var result = await _controller.GetMessageFileById(chatId, fileId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<FileStreamResult>()
            .Which.Should().BeEquivalentTo(new
            {
                FileStream = fileStream,
                ContentType = contentType
            });
        _mediatorMock.Verify(
            m => m.Send(It.Is<GetMessageFileByIdQuery>(q => q.ChatId == chatId && q.FileId == fileId), It.IsAny<CancellationToken>()),
            Times.Once());
    }
}