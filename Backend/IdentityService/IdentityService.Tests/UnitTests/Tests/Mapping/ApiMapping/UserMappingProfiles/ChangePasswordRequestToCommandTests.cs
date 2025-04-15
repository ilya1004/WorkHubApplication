using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.UserMappingProfiles;

public class ChangePasswordRequestToCommandTests
{
    private readonly IMapper _mapper;

    public ChangePasswordRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ChangePasswordRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapChangePasswordRequestToCommand()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "oldPassword", "newPassword");

        // Act
        var command = _mapper.Map<ChangePasswordCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(request.Email);
        command.CurrentPassword.Should().Be(request.CurrentPassword);
        command.NewPassword.Should().Be(request.NewPassword);
    }
}