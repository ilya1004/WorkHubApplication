using IdentityService.BLL.Mapping.FreelancerSkillMappingProfiles;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.FreelancerSkillMappingProfiles;

public class UpdateFreelancerSkillCommandToFreelancerSkillProfileTests
{
    private readonly IMapper _mapper;

    public UpdateFreelancerSkillCommandToFreelancerSkillProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<UpdateFreelancerSkillCommandToFreelancerSkillProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapUpdateFreelancerSkillCommandToFreelancerSkill()
    {
        // Arrange
        var command = new UpdateFreelancerSkillCommand(Guid.NewGuid(), "Python Programming");

        // Act
        var freelancerSkill = _mapper.Map<FreelancerSkill>(command);

        // Assert
        freelancerSkill.Should().NotBeNull();
        freelancerSkill.Name.Should().Be("Python Programming");
        freelancerSkill.NormalizedName.Should().Be("PYTHON_PROGRAMMING");
        freelancerSkill.FreelancerProfiles.Should().BeNull();
    }

    [Fact]
    public void ShouldMapUpdateFreelancerSkillCommand_WithSpacesInName()
    {
        // Arrange
        var command = new UpdateFreelancerSkillCommand(Guid.NewGuid(), "Data Analysis");

        // Act
        var freelancerSkill = _mapper.Map<FreelancerSkill>(command);

        // Assert
        freelancerSkill.Should().NotBeNull();
        freelancerSkill.Name.Should().Be("Data Analysis");
        freelancerSkill.NormalizedName.Should().Be("DATA_ANALYSIS");
        freelancerSkill.FreelancerProfiles.Should().BeNull();
    }
}