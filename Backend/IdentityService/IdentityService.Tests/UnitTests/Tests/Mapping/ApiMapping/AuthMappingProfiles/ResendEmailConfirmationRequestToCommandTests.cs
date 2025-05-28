using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Mapping.AuthMappingProfiles;
using IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.AuthMappingProfiles;

public class ResendEmailConfirmationRequestToCommandTests
{
    private readonly IMapper _mapper;

    public ResendEmailConfirmationRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ResendEmailConfirmationRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapResendEmailConfirmationRequestToCommand()
    {
        // Arrange
        var request = new ResendEmailConfirmationRequest("user@example.com");

        // Act
        var command = _mapper.Map<ResendEmailConfirmationCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Email.Should().Be(request.Email);
    }
}