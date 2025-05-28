using IdentityService.BLL.Mapping.FreelancerSkillMappingProfiles;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.FreelancerSkillMappingProfiles;

public class CreateFreelancerSkillCommandToFreelancerSkillProfileTests
{
    private readonly IMapper _mapper;

    public CreateFreelancerSkillCommandToFreelancerSkillProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CreateFreelancerSkillCommandToFreelancerSkillProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapCreateFreelancerSkillCommandToFreelancerSkill()
    {
        // Arrange
        var command = new CreateFreelancerSkillCommand("C# Programming");

        // Act
        var freelancerSkill = _mapper.Map<FreelancerSkill>(command);

        // Assert
        freelancerSkill.Should().NotBeNull();
        freelancerSkill.Name.Should().Be("C# Programming");
        freelancerSkill.NormalizedName.Should().Be("C#_PROGRAMMING");
        freelancerSkill.FreelancerProfiles.Should().BeNull();
    }

    [Fact]
    public void ShouldMapCreateFreelancerSkillCommand_WithSpacesInName()
    {
        // Arrange
        var command = new CreateFreelancerSkillCommand("Web Development");

        // Act
        var freelancerSkill = _mapper.Map<FreelancerSkill>(command);

        // Assert
        freelancerSkill.Should().NotBeNull();
        freelancerSkill.Name.Should().Be("Web Development");
        freelancerSkill.NormalizedName.Should().Be("WEB_DEVELOPMENT");
        freelancerSkill.FreelancerProfiles.Should().BeNull();
    }
}