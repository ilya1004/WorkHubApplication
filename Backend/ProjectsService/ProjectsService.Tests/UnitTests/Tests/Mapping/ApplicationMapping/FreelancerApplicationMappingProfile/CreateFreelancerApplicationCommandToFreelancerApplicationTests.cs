using ProjectsService.Application.Mapping.FreelancerApplicationMappingProfile;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

namespace ProjectsService.Tests.UnitTests.Tests.Mapping.ApplicationMapping.FreelancerApplicationMappingProfile;

public class CreateFreelancerApplicationCommandToFreelancerApplicationTests
{
    private readonly IMapper _mapper;

    public CreateFreelancerApplicationCommandToFreelancerApplicationTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateFreelancerApplicationCommandToFreelancerApplication>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_ValidCommand_MapsToFreelancerApplicationCorrectly()
    {
        // Arrange
        var command = new CreateFreelancerApplicationCommand(Guid.NewGuid());
        var mappingTime = DateTime.UtcNow;

        // Act
        var application = _mapper.Map<FreelancerApplication>(command);

        // Assert
        application.Should().NotBeNull();
        application.Id.Should().NotBe(Guid.Empty);
        application.ProjectId.Should().Be(command.ProjectId);
        application.Status.Should().Be(ApplicationStatus.Pending);
        application.CreatedAt.Should().BeCloseTo(mappingTime, TimeSpan.FromSeconds(1));
        application.Project.Should().BeNull();
        application.FreelancerId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Map_EmptyProjectId_MapsToFreelancerApplicationWithEmptyProjectId()
    {
        // Arrange
        var command = new CreateFreelancerApplicationCommand(Guid.Empty);
        var mappingTime = DateTime.UtcNow;

        // Act
        var application = _mapper.Map<FreelancerApplication>(command);

        // Assert
        application.Should().NotBeNull();
        application.Id.Should().NotBe(Guid.Empty);
        application.ProjectId.Should().Be(Guid.Empty);
        application.Status.Should().Be(ApplicationStatus.Pending);
        application.CreatedAt.Should().BeCloseTo(mappingTime, TimeSpan.FromSeconds(1));
        application.Project.Should().BeNull();
        application.FreelancerId.Should().Be(Guid.Empty);
    }
}