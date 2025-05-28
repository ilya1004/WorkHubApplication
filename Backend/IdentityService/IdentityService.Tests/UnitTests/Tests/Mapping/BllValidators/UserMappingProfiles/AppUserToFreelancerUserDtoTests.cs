using IdentityService.BLL.Mapping.UserMappingProfiles;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.UserMappingProfiles;

public class AppUserToFreelancerUserDtoTests
{
    private readonly IMapper _mapper;

    public AppUserToFreelancerUserDtoTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AppUserToFreelancerUserDto>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapAppUserToFreelancerUserDto()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "freelancer",
            Email = "freelancer@example.com",
            RegisteredAt = DateTime.UtcNow,
            ImageUrl = "https://image.url",
            FreelancerProfile = new FreelancerProfile
            {
                FirstName = "John",
                LastName = "Doe",
                About = "About freelancer",
                StripeAccountId = "acct_123",
                Skills = new List<FreelancerSkill>
                {
                    new FreelancerSkill { Id = skillId, Name = "C#" }
                }
            },
            Role = new IdentityRole<Guid> { Name = "Freelancer" }
        };

        // Act
        var dto = _mapper.Map<FreelancerUserDto>(appUser);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(appUser.Id.ToString());
        dto.UserName.Should().Be(appUser.UserName);
        dto.FirstName.Should().Be(appUser.FreelancerProfile.FirstName);
        dto.LastName.Should().Be(appUser.FreelancerProfile.LastName);
        dto.About.Should().Be(appUser.FreelancerProfile.About);
        dto.Email.Should().Be(appUser.Email);
        dto.RegisteredAt.Should().Be(appUser.RegisteredAt);
        dto.StripeAccountId.Should().Be(appUser.FreelancerProfile.StripeAccountId);
        dto.Skills.Should().BeEquivalentTo(new List<FreelancerSkillDto>
        {
            new FreelancerSkillDto(skillId.ToString(), "C#")
        });
        dto.ImageUrl.Should().Be(appUser.ImageUrl);
        dto.RoleName.Should().Be(appUser.Role.Name);
    }

    [Fact]
    public void ShouldMapAppUserToFreelancerUserDto_WithNullOptionalFields()
    {
        // Arrange
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "freelancer",
            Email = "freelancer@example.com",
            RegisteredAt = DateTime.UtcNow,
            ImageUrl = null,
            FreelancerProfile = new FreelancerProfile
            {
                FirstName = "John",
                LastName = "Doe",
                About = null,
                StripeAccountId = null,
                Skills = new List<FreelancerSkill>()
            },
            Role = new IdentityRole<Guid> { Name = "Freelancer" }
        };

        // Act
        var dto = _mapper.Map<FreelancerUserDto>(appUser);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(appUser.Id.ToString());
        dto.UserName.Should().Be(appUser.UserName);
        dto.FirstName.Should().Be(appUser.FreelancerProfile.FirstName);
        dto.LastName.Should().Be(appUser.FreelancerProfile.LastName);
        dto.About.Should().BeNull();
        dto.Email.Should().Be(appUser.Email);
        dto.RegisteredAt.Should().Be(appUser.RegisteredAt);
        dto.StripeAccountId.Should().BeNull();
        dto.Skills.Should().BeEmpty();
        dto.ImageUrl.Should().BeNull();
        dto.RoleName.Should().Be(appUser.Role.Name);
    }
}