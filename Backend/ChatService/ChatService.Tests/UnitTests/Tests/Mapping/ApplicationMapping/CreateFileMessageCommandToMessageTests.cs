using ChatService.Application.Mapping.MessageMappingProfiles;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApplicationMapping;

public class CreateFileMessageCommandToMessageTests
{
    private readonly IMapper _mapper;

    public CreateFileMessageCommandToMessageTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateFileMessageCommandToMessage>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_CreateFileMessageCommandToMessage_ShouldMapPropertiesAndSetDefaults()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var fileStream = new MemoryStream(new byte[] { 1, 2, 3 });
        var contentType = "application/pdf";

        var command = new CreateFileMessageCommand(
            ChatId: chatId,
            ReceiverId: receiverId,
            FileStream: fileStream,
            ContentType: contentType);

        // Act
        var message = _mapper.Map<Message>(command);

        // Assert
        message.Should().NotBeNull();
        message.ChatId.Should().Be(command.ChatId);
        message.ReceiverId.Should().Be(command.ReceiverId);
        message.Text.Should().BeNull();
        message.FileId.Should().BeNull();
        message.SenderId.Should().Be(Guid.Empty);
        message.Type.Should().Be(MessageType.File);
        message.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}