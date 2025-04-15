using IdentityService.BLL.Mapping.EmployerIndustryMappingProfiles;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.EmployerIndustryMappingProfiles;

public class UpdateEmployerIndustryCommandToEmployerIndustryProfileTests
{
    private readonly IMapper _mapper;

    public UpdateEmployerIndustryCommandToEmployerIndustryProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<UpdateEmployerIndustryCommandToEmployerIndustryProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapUpdateEmployerIndustryCommandToEmployerIndustry()
    {
        // Arrange
        var command = new UpdateEmployerIndustryCommand(Guid.NewGuid(), "Software Engineering");

        // Act
        var employerIndustry = _mapper.Map<EmployerIndustry>(command);

        // Assert
        employerIndustry.Should().NotBeNull();
        employerIndustry.Name.Should().Be("Software Engineering");
        employerIndustry.NormalizedName.Should().Be("SOFTWARE_ENGINEERING");
        employerIndustry.EmployerProfiles.Should().BeNull();
    }

    [Fact]
    public void ShouldMapUpdateEmployerIndustryCommand_WithSpacesInName()
    {
        // Arrange
        var command = new UpdateEmployerIndustryCommand(Guid.NewGuid(), "Machine Learning");

        // Act
        var employerIndustry = _mapper.Map<EmployerIndustry>(command);

        // Assert
        employerIndustry.Should().NotBeNull();
        employerIndustry.Name.Should().Be("Machine Learning");
        employerIndustry.NormalizedName.Should().Be("MACHINE_LEARNING");
        employerIndustry.EmployerProfiles.Should().BeNull();
    }
}