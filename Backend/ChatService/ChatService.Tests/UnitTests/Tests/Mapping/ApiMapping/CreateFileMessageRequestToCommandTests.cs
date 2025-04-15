using ChatService.API.Contracts.ChatContracts;
using ChatService.API.Mapping.MessageMappingProfiles;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using Microsoft.AspNetCore.Http;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApiMapping;

public class CreateFileMessageRequestToCommandTests
{
    private readonly IMapper _mapper;

    public CreateFileMessageRequestToCommandTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateFileMessageRequestToCommand>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_CreateFileMessageRequestToCommand_ShouldMapAllPropertiesAndConstructStream()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var contentType = "application/pdf";
        var fileStream = new MemoryStream(new byte[] { 1, 2, 3 });

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(fileStream);
        fileMock.Setup(f => f.ContentType).Returns(contentType);

        var request = new CreateFileMessageRequest(
            ChatId: chatId,
            ReceiverId: receiverId,
            File: fileMock.Object);

        // Act
        var command = _mapper.Map<CreateFileMessageCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.ChatId.Should().Be(request.ChatId);
        command.ReceiverId.Should().Be(request.ReceiverId);
        command.FileStream.Should().BeSameAs(fileStream);
        command.ContentType.Should().Be(contentType);
    }
}