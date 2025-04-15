using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Mapping.AuthMappingProfiles;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.AuthMappingProfiles;

public class RefreshTokenRequestToCommandTests
{
    private readonly IMapper _mapper;

    public RefreshTokenRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RefreshTokenRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRefreshTokenRequestToCommand()
    {
        // Arrange
        var request = new RefreshTokenRequest("access_token", "refresh_token");

        // Act
        var command = _mapper.Map<RefreshTokenCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.AccessToken.Should().Be(request.AccessToken);
        command.RefreshToken.Should().Be(request.RefreshToken);
    }
}