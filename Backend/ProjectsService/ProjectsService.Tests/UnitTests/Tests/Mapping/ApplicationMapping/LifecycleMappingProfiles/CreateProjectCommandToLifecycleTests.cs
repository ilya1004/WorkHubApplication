using ProjectsService.Application.Mapping.LifecycleMappingProfiles;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.LifecycleMappingProfiles;

public class CreateProjectCommandToLifecycleTests
{
    private readonly IMapper _mapper;

    public CreateProjectCommandToLifecycleTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateProjectCommandToLifecycle>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidCommand_MapsToLifecycleCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var command = new CreateProjectCommand(
            Project: new ProjectDto(
                Title: "Test Project",
                Description: "Description",
                Budget: 1000.50m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: now.AddDays(1),
                ApplicationsDeadline: now.AddDays(2),
                WorkStartDate: now.AddDays(3),
                WorkDeadline: now.AddDays(4)));

        // Act
        var lifecycle = _mapper.Map<Lifecycle>(command);

        // Assert
        lifecycle.Should().NotBeNull();
        lifecycle.Id.Should().NotBe(Guid.Empty);
        lifecycle.CreatedAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        lifecycle.UpdatedAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        lifecycle.ApplicationsStartDate.Should().Be(command.Lifecycle.ApplicationsStartDate);
        lifecycle.ApplicationsDeadline.Should().Be(command.Lifecycle.ApplicationsDeadline);
        lifecycle.WorkStartDate.Should().Be(command.Lifecycle.WorkStartDate);
        lifecycle.WorkDeadline.Should().Be(command.Lifecycle.WorkDeadline);
        lifecycle.Status.Should().Be(ProjectStatus.Published);
        lifecycle.ProjectId.Should().Be(Guid.Empty);
        lifecycle.Project.Should().BeNull();
        lifecycle.AcceptanceRequested.Should().BeFalse();
        lifecycle.AcceptanceConfirmed.Should().BeFalse();
    }
}