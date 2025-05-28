using ChatService.Application.Mapping.ChatMappingProfiles;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApplicationMapping;

public class CreateChatCommandToChatTests
{
    private readonly IMapper _mapper;

    public CreateChatCommandToChatTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateChatCommandToChat>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_CreateChatCommandToChat_ShouldMapPropertiesAndSetDefaults()
    {
        // Arrange
        var command = new CreateChatCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        var chat = _mapper.Map<Chat>(command);

        // Assert
        chat.Should().NotBeNull();
        chat.EmployerId.Should().Be(command.EmployerId);
        chat.FreelancerId.Should().Be(command.FreelancerId);
        chat.ProjectId.Should().Be(command.ProjectId);
        chat.Id.Should().NotBeEmpty();
        chat.IsActive.Should().BeTrue();
        chat.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}