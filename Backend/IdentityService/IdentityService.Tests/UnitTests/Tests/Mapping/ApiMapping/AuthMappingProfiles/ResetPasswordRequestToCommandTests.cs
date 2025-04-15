using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Mapping.AuthMappingProfiles;
using IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.AuthMappingProfiles;

public class ResetPasswordRequestToCommandTests
{
    private readonly IMapper _mapper;

    public ResetPasswordRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ResetPasswordRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapResetPasswordRequestToCommand()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "newPassword123", "code123");

        // Act
        var command = _mapper.Map<ResetPasswordCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(request.Email);
        command.NewPassword.Should().Be(request.NewPassword);
        command.Code.Should().Be(request.Code);
    }
}