using ProjectsService.Application.Mapping.ProjectMappingProfiles;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.ProjectMappingProfiles;

public class ProjectDtoToProjectTests
{
    private readonly IMapper _mapper;

    public ProjectDtoToProjectTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProjectDtoToProject>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidDto_MapsToProjectCorrectly()
    {
        // Arrange
        var dto = new ProjectDto(
            Title: "Test Project",
            Description: "Description",
            Budget: 1000.50m,
            CategoryId: Guid.NewGuid());

        // Act
        var project = _mapper.Map<Project>(dto);

        // Assert
        project.Should().NotBeNull();
        project.Id.Should().NotBe(Guid.Empty);
        project.Title.Should().Be(dto.Title);
        project.Description.Should().Be(dto.Description);
        project.Budget.Should().Be(dto.Budget);
        project.CategoryId.Should().Be(dto.CategoryId);
        project.Category.Should().BeNull();
        project.FreelancerApplications.Should().BeNull();
        project.EmployerId.Should().Be(Guid.Empty);
        project.FreelancerId.Should().BeNull();
        project.PaymentIntentId.Should().BeNull();
        project.Lifecycle.Should().BeNull();
    }

    [Fact]
    public void Map_NullDescription_MapsToProjectWithNullDescription()
    {
        // Arrange
        var dto = new ProjectDto(
            Title: "Test Project",
            Description: null,
            Budget: 1000.50m,
            CategoryId: null);

        // Act
        var project = _mapper.Map<Project>(dto);

        // Assert
        project.Should().NotBeNull();
        project.Id.Should().NotBe(Guid.Empty);
        project.Title.Should().Be(dto.Title);
        project.Description.Should().BeNull();
        project.Budget.Should().Be(dto.Budget);
        project.CategoryId.Should().BeNull();
        project.Category.Should().BeNull();
        project.FreelancerApplications.Should().BeNull();
        project.EmployerId.Should().Be(Guid.Empty);
        project.FreelancerId.Should().BeNull();
        project.PaymentIntentId.Should().BeNull();
        project.Lifecycle.Should().BeNull();
    }
}