using ChatService.API.Contracts.ChatContracts;
using ChatService.API.Mapping.MessageMappingProfiles;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApiMapping;

public class CreateTextMessageRequestToCommandTests
{
    private readonly IMapper _mapper;

    public CreateTextMessageRequestToCommandTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateTextMessageRequestToCommand>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_CreateTextMessageRequestToCommand_ShouldMapAllProperties()
    {
        // Arrange
        var request = new CreateTextMessageRequest(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            Text: "Hello, world!");

        // Act
        var command = _mapper.Map<CreateTextMessageCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.ChatId.Should().Be(request.ChatId);
        command.ReceiverId.Should().Be(request.ReceiverId);
        command.Text.Should().Be(request.Text);
    }
}