using ProjectsService.Application.Mapping.LifecycleMappingProfiles;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.LifecycleMappingProfiles;

public class LifecycleDtoToLifecycleTests
{
    private readonly IMapper _mapper;

    public LifecycleDtoToLifecycleTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LifecycleDtoToLifecycle>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidDto_MapsToLifecycleCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dto = new LifecycleDto(
            ApplicationsStartDate: now.AddDays(1),
            ApplicationsDeadline: now.AddDays(2),
            WorkStartDate: now.AddDays(3),
            WorkDeadline: now.AddDays(4));

        // Act
        var lifecycle = _mapper.Map<Lifecycle>(dto);

        // Assert
        lifecycle.Should().NotBeNull();
        lifecycle.ApplicationsStartDate.Should().Be(dto.ApplicationsStartDate);
        lifecycle.ApplicationsDeadline.Should().Be(dto.ApplicationsDeadline);
        lifecycle.WorkStartDate.Should().Be(dto.WorkStartDate);
        lifecycle.WorkDeadline.Should().Be(dto.WorkDeadline);
        lifecycle.UpdatedAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        lifecycle.Id.Should().Be(Guid.Empty);
        lifecycle.CreatedAt.Should().Be(default);
        lifecycle.Status.Should().Be(default);
        lifecycle.ProjectId.Should().Be(Guid.Empty);
        lifecycle.Project.Should().BeNull();
        lifecycle.AcceptanceRequested.Should().BeFalse();
        lifecycle.AcceptanceConfirmed.Should().BeFalse();
    }
}