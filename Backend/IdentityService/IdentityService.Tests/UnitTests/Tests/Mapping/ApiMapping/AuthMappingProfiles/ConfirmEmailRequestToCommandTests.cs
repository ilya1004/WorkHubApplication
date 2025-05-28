using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Mapping.AuthMappingProfiles;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.AuthMappingProfiles;

public class ConfirmEmailRequestToCommandTests
{
    private readonly IMapper _mapper;

    public ConfirmEmailRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ConfirmEmailRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapConfirmEmailRequestToCommand()
    {
        // Arrange
        var request = new ConfirmEmailRequest("user@example.com", "123456");

        // Act
        var command = _mapper.Map<ConfirmEmailCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(request.Email);
        command.Code.Should().Be(request.Code);
    }
}