using IdentityService.BLL.Mapping.EmployerIndustryMappingProfiles;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

namespace IdentityService.Tests.UnitTests.Tests.Mapping.BllValidators.EmployerIndustryMappingProfiles;

public class CreateEmployerIndustryCommandToEmployerIndustryProfileTests
{
    private readonly IMapper _mapper;

    public CreateEmployerIndustryCommandToEmployerIndustryProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CreateEmployerIndustryCommandToEmployerIndustryProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void ShouldMapCreateEmployerIndustryCommandToEmployerIndustry()
    {
        // Arrange
        var command = new CreateEmployerIndustryCommand("Software Development");

        // Act
        var employerIndustry = _mapper.Map<EmployerIndustry>(command);

        // Assert
        employerIndustry.Should().NotBeNull();
        employerIndustry.Name.Should().Be("Software Development");
        employerIndustry.NormalizedName.Should().Be("SOFTWARE_DEVELOPMENT");
        employerIndustry.EmployerProfiles.Should().BeNull();
    }

    [Fact]
    public void ShouldMapCreateEmployerIndustryCommand_WithSpacesInName()
    {
        // Arrange
        var command = new CreateEmployerIndustryCommand("Data Science");

        // Act
        var employerIndustry = _mapper.Map<EmployerIndustry>(command);

        // Assert
        employerIndustry.Should().NotBeNull();
        employerIndustry.Name.Should().Be("Data Science");
        employerIndustry.NormalizedName.Should().Be("DATA_SCIENCE");
        employerIndustry.EmployerProfiles.Should().BeNull();
    }
}