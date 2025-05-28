using ProjectsService.Application.Mapping.ProjectMappingProfiles;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.ProjectMappingProfiles;

public class UpdateProjectDtoToProjectTests
{
    private readonly IMapper _mapper;

    public UpdateProjectDtoToProjectTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UpdateProjectDtoToProject>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidDto_MapsToProjectCorrectly()
    {
        // Arrange
        var dto = new UpdateProjectDto(
            Title: "Updated Project",
            Description: "Updated Description",
            Budget: 2000.75m,
            CategoryId: Guid.NewGuid());

        // Act
        var project = _mapper.Map<Project>(dto);

        // Assert
        project.Should().NotBeNull();
        project.Title.Should().Be(dto.Title);
        project.Description.Should().Be(dto.Description);
        project.Budget.Should().Be(dto.Budget);
        project.CategoryId.Should().Be(dto.CategoryId);
        project.Id.Should().Be(Guid.Empty);
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
        var dto = new UpdateProjectDto(
            Title: "Updated Project",
            Description: null,
            Budget: 2000.75m,
            CategoryId: null);

        // Act
        var project = _mapper.Map<Project>(dto);

        // Assert
        project.Should().NotBeNull();
        project.Title.Should().Be(dto.Title);
        project.Description.Should().BeNull();
        project.Budget.Should().Be(dto.Budget);
        project.CategoryId.Should().BeNull();
        project.Id.Should().Be(Guid.Empty);
        project.Category.Should().BeNull();
        project.FreelancerApplications.Should().BeNull();
        project.EmployerId.Should().Be(Guid.Empty);
        project.FreelancerId.Should().BeNull();
        project.PaymentIntentId.Should().BeNull();
        project.Lifecycle.Should().BeNull();
    }
}