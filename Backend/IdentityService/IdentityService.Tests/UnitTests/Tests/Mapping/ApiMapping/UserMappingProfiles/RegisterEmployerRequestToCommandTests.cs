using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.ApiMapping.UserMappingProfiles;

public class RegisterEmployerRequestToCommandTests
{
    private readonly IMapper _mapper;

    public RegisterEmployerRequestToCommandTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RegisterEmployerRequestToCommand>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRegisterEmployerRequestToCommand()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "password123");

        // Act
        var command = _mapper.Map<RegisterEmployerCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.UserName.Should().Be(request.UserName);
        command.CompanyName.Should().Be(request.CompanyName);
        command.Email.Should().Be(request.Email);
        command.Password.Should().Be(request.Password);
    }
}