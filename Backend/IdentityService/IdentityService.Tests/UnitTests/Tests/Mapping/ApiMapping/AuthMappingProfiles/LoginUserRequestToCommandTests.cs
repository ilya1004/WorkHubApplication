using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Mapping.AuthMappingProfiles;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.AuthMappingProfiles;

public class LoginUserRequestToCommandTests
{
    private readonly IMapper _mapper;

    public LoginUserRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<LoginUserRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapLoginUserRequestToCommand()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "password123");

        // Act
        var command = _mapper.Map<LoginUserCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(request.Email);
        command.Password.Should().Be(request.Password);
    }
}