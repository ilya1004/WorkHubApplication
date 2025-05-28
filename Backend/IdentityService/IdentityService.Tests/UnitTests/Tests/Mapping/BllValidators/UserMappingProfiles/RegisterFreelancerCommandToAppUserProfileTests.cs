using IdentityService.BLL.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class RegisterFreelancerCommandToAppUserProfileTests
{
    private readonly IMapper _mapper;

    public RegisterFreelancerCommandToAppUserProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RegisterFreelancerCommandToAppUserProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRegisterFreelancerCommandToAppUser()
    {
        // Arrange
        var command = new RegisterFreelancerCommand("freelancer", "John", "Doe", "john@example.com", "P@ssw0rd123");

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