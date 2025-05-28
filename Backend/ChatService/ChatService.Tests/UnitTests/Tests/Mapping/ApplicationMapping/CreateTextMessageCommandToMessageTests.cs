using ChatService.Application.Mapping.MessageMappingProfiles;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApplicationMapping;

public class CreateTextMessageCommandToMessageTests
{
    private readonly IMapper _mapper;

    public CreateTextMessageCommandToMessageTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateTextMessageCommandToMessage>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_CreateTextMessageCommandToMessage_ShouldMapPropertiesAndSetDefaults()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Hello, world!");

        // Act
        var message = _mapper.Map<Message>(command);

        // Assert
        message.Should().NotBeNull();
        message.ChatId.Should().Be(command.ChatId);
        message.ReceiverId.Should().Be(command.ReceiverId);
        message.Text.Should().Be(command.Text);
        message.FileId.Should().BeNull();
        message.SenderId.Should().Be(Guid.Empty);
        message.Type.Should().Be(MessageType.Text);
        message.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}