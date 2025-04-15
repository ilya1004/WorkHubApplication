using IdentityService.BLL.Mapping.UserMappingProfiles;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class RegisterFreelancerCommandToFreelancerProfileProfileTests
{
    private readonly IMapper _mapper;

    public RegisterFreelancerCommandToFreelancerProfileProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<RegisterFreelancerCommandToFreelancerProfileProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapRegisterFreelancerCommandToFreelancerProfile()
    {
        // Arrange
        var command = new RegisterFreelancerCommand("freelancer", "John", "Doe", "john@example.com", "P@ssw0rd123");

        // Act
        var freelancerProfile = _mapper.Map<FreelancerProfile>(command);

        // Assert
        freelancerProfile.Should().NotBeNull();
        freelancerProfile.FirstName.Should().Be(command.FirstName);
        freelancerProfile.LastName.Should().Be(command.LastName);
    }
}