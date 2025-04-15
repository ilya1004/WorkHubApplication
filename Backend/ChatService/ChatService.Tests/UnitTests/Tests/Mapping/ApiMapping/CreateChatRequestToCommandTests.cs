using ChatService.API.Contracts.ChatContracts;
using ChatService.API.Mapping.ChatMappingProfiles;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApiMapping;

public class CreateChatRequestToCommandTests
{
    private readonly IMapper _mapper;

    public CreateChatRequestToCommandTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CreateChatRequestToCommand>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_CreateChatRequestToCommand_ShouldMapAllProperties()
    {
        // Arrange
        var request = new CreateChatRequest(
            EmployerId: Guid.NewGuid(),
            FreelancerId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid());

        // Act
        var command = _mapper.Map<CreateChatCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.EmployerId.Should().Be(request.EmployerId);
        command.FreelancerId.Should().Be(request.FreelancerId);
        command.ProjectId.Should().Be(request.ProjectId);
    }
}