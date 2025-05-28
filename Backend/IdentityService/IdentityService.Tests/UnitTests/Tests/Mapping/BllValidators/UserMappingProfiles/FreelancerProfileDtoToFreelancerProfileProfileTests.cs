using IdentityService.BLL.Mapping.UserMappingProfiles;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class FreelancerProfileDtoToFreelancerProfileProfileTests
{
    private readonly IMapper _mapper;

    public FreelancerProfileDtoToFreelancerProfileProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<FreelancerProfileDtoToFreelancerProfileProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapFreelancerProfileDtoToFreelancerProfile()
    {
        // Arrange
        var skillIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var dto = new FreelancerProfileDto("John", "Doe", "About freelancer", skillIds, true);

        // Act
        var freelancerProfile = _mapper.Map<FreelancerProfile>(dto);

        // Assert
        freelancerProfile.Should().NotBeNull();
        freelancerProfile.FirstName.Should().Be(dto.FirstName);
        freelancerProfile.LastName.Should().Be(dto.LastName);
        freelancerProfile.About.Should().Be(dto.About);
        freelancerProfile.Skills.Should().BeNull();
        freelancerProfile.Id.Should().Be(Guid.Empty);
        freelancerProfile.UserId.Should().Be(Guid.Empty);
        freelancerProfile.User.Should().BeNull();
        freelancerProfile.StripeAccountId.Should().BeNull();
    }

    [Fact]
    public void ShouldMapFreelancerProfileDtoToFreelancerProfile_WithNullOptionalFields()
    {
        // Arrange
        var dto = new FreelancerProfileDto("John", "Doe", null, null, false);

        // Act
        var freelancerProfile = _mapper.Map<FreelancerProfile>(dto);

        // Assert
        freelancerProfile.Should().NotBeNull();
        freelancerProfile.FirstName.Should().Be(dto.FirstName);
        freelancerProfile.LastName.Should().Be(dto.LastName);
        freelancerProfile.About.Should().BeNull();
        freelancerProfile.Skills.Should().BeNull();
        freelancerProfile.Id.Should().Be(Guid.Empty);
        freelancerProfile.UserId.Should().Be(Guid.Empty);
        freelancerProfile.User.Should().BeNull();
        freelancerProfile.StripeAccountId.Should().BeNull();
    }
}