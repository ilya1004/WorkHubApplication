using IdentityService.BLL.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class RegisterEmployerCommandToAppUserProfileTests
{
    private readonly IMapper _mapper;

    public RegisterEmployerCommandToAppUserProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RegisterEmployerCommandToAppUserProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRegisterEmployerCommandToAppUser()
    {
        // Arrange
        var command = new RegisterEmployerCommand("company", "Company Inc", "company@example.com", "P@ssw0rd123");

        // Act
        var appUser = _mapper.Map<AppUser>(command);

        // Assert
        appUser.Should().NotBeNull();
        appUser.UserName.Should().Be(command.UserName);
        appUser.Email.Should().Be(command.Email);
        appUser.EmailConfirmed.Should().BeFalse();
        appUser.RegisteredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}