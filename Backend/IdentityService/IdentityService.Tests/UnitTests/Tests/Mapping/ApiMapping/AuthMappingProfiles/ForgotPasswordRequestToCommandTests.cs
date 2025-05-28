using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Mapping.AuthMappingProfiles;
using IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.AuthMappingProfiles;

public class ForgotPasswordRequestToCommandTests
{
    private readonly IMapper _mapper;

    public ForgotPasswordRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ForgotPasswordRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapForgotPasswordRequestToCommand()
    {
        // Arrange
        var request = new ForgotPasswordRequest("user@example.com", "http://reset.url");

        // Act
        var command = _mapper.Map<ForgotPasswordCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(request.Email);
        command.ResetUrl.Should().Be(request.ResetUrl);
    }
}