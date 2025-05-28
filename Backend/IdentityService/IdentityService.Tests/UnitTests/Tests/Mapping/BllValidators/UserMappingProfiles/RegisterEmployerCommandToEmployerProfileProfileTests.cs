using IdentityService.BLL.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class RegisterEmployerCommandToEmployerProfileProfileTests
{
    private readonly IMapper _mapper;

    public RegisterEmployerCommandToEmployerProfileProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RegisterEmployerCommandToEmployerProfileProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRegisterEmployerCommandToEmployerProfile()
    {
        // Arrange
        var command = new RegisterEmployerCommand("company", "Company Inc", "company@example.com", "P@ssw0rd123");

        // Act
        var employerProfile = _mapper.Map<EmployerProfile>(command);

        // Assert
        employerProfile.Should().NotBeNull();
        employerProfile.CompanyName.Should().Be(command.CompanyName);
    }
}