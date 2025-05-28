using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;
using ProjectsService.Application.Settings;
using ProjectsService.Domain.Abstractions.Data;

namespace ProjectsService.Tests.UnitTests.Tests.BackgroundJobs;

public class UpdateProjectStatusesCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IQueriesRepository<Project>> _projectQueriesRepositoryMock;
    private readonly Mock<ICommandsRepository<Lifecycle>> _lifecycleCommandsRepositoryMock;
    private readonly Mock<ICommandsRepository<Project>> _projectCommandsRepositoryMock;
    private readonly Mock<IOptions<ProjectsSettings>> _optionsMock;
    private readonly Mock<ILogger<UpdateProjectStatusesCommandHandler>> _loggerMock;
    private readonly UpdateProjectStatusesCommandHandler _handler;
    private readonly ProjectsSettings _settings;

    public UpdateProjectStatusesCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _projectQueriesRepositoryMock = new Mock<IQueriesRepository<Project>>();
        _lifecycleCommandsRepositoryMock = new Mock<ICommandsRepository<Lifecycle>>();
        _projectCommandsRepositoryMock = new Mock<ICommandsRepository<Project>>();
        _optionsMock = new Mock<IOptions<ProjectsSettings>>();
        _loggerMock = new Mock<ILogger<UpdateProjectStatusesCommandHandler>>();

        _settings = new ProjectsSettings { MaxWorkDeadlineExpirationTimeInDays = 30 };
        _optionsMock.Setup(x => x.Value).Returns(_settings);

        _unitOfWorkMock.Setup(x => x.ProjectQueriesRepository).Returns(_projectQueriesRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.LifecycleCommandsRepository).Returns(_lifecycleCommandsRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.ProjectCommandsRepository).Returns(_projectCommandsRepositoryMock.Object);

        _handler = new UpdateProjectStatusesCommandHandler(
            _unitOfWorkMock.Object,
            _optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoProjects_DoesNotCallRepositories()
    {
        // Arrange
        var command = new UpdateProjectStatusesCommand();
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(new List<Project>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()),
            Times.Never());
        _projectCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
            Times.Never());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_AcceptanceConfirmed_UpdatesStatusToCompleted()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.InProgress,
                ApplicationsStartDate = DateTime.UtcNow.AddDays(-10),
                ApplicationsDeadline = DateTime.UtcNow.AddDays(-5),
                WorkStartDate = DateTime.UtcNow.AddDays(-2),
                WorkDeadline = DateTime.UtcNow.AddDays(2),
                AcceptanceConfirmed = true,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.Completed);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_ExpiredWorkDeadlineAndOldUpdate_UpdatesStatusToCancelled()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.Expired,
                ApplicationsStartDate = now.AddDays(-20),
                ApplicationsDeadline = now.AddDays(-15),
                WorkStartDate = now.AddDays(-10),
                WorkDeadline = now.AddDays(-5),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-_settings.MaxWorkDeadlineExpirationTimeInDays + 1)
            },
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.Cancelled);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_PastWorkDeadline_UpdatesStatusToExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.InProgress,
                ApplicationsStartDate = now.AddDays(-20),
                ApplicationsDeadline = now.AddDays(-15),
                WorkStartDate = now.AddDays(-10),
                WorkDeadline = now.AddDays(-1),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-1)
            },
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.Expired);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_PastWorkStartDateNoFreelancer_UpdatesStatusToCancelled()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.WaitingForWorkStart,
                ApplicationsStartDate = now.AddDays(-10),
                ApplicationsDeadline = now.AddDays(-5),
                WorkStartDate = now.AddDays(-1),
                WorkDeadline = now.AddDays(5),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-1)
            },
            FreelancerId = null,
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.Cancelled);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_PastWorkStartDateWithAcceptedApplication_UpdatesStatusToInProgress()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var freelancerId = Guid.NewGuid();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.WaitingForWorkStart,
                ApplicationsStartDate = now.AddDays(-10),
                ApplicationsDeadline = now.AddDays(-5),
                WorkStartDate = now.AddDays(-1),
                WorkDeadline = now.AddDays(5),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-1)
            },
            FreelancerApplications = new List<FreelancerApplication>
            {
                new FreelancerApplication { FreelancerId = freelancerId, Status = ApplicationStatus.Accepted },
                new FreelancerApplication { FreelancerId = Guid.NewGuid(), Status = ApplicationStatus.Pending }
            }
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.InProgress);
        project.FreelancerId.Should().Be(freelancerId);
        project.FreelancerApplications.First(a => a.FreelancerId == freelancerId).Status.Should().Be(ApplicationStatus.Accepted);
        project.FreelancerApplications.First(a => a.FreelancerId != freelancerId).Status.Should().Be(ApplicationStatus.Rejected);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _projectCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_PastApplicationsDeadline_UpdatesStatusToWaitingForWorkStart()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.AcceptingApplications,
                ApplicationsStartDate = now.AddDays(-5),
                ApplicationsDeadline = now.AddDays(-1),
                WorkStartDate = now.AddDays(5),
                WorkDeadline = now.AddDays(10),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-1)
            },
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.WaitingForWorkStart);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_PastApplicationsStartDate_UpdatesStatusToAcceptingApplications()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.PendingForReview,
                ApplicationsStartDate = now.AddDays(-1),
                ApplicationsDeadline = now.AddDays(5),
                WorkStartDate = now.AddDays(10),
                WorkDeadline = now.AddDays(15),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-1)
            },
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        project.Lifecycle.Status.Should().Be(ProjectStatus.AcceptingApplications);
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(project.Lifecycle, It.IsAny<CancellationToken>()),
            Times.Once());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_NoStatusChange_DoesNotUpdateLifecycle()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.AcceptingApplications,
                ApplicationsStartDate = now.AddDays(-1),
                ApplicationsDeadline = now.AddDays(5),
                WorkStartDate = now.AddDays(10),
                WorkDeadline = now.AddDays(15),
                AcceptanceConfirmed = false,
                UpdatedAt = now.AddDays(-1)
            },
            FreelancerApplications = []
        };
        var projects = new List<Project> { project };
        _projectQueriesRepositoryMock
            .Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        // Act
        await _handler.Handle(new UpdateProjectStatusesCommand(), CancellationToken.None);

        // Assert
        _lifecycleCommandsRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()),
            Times.Never());
        _unitOfWorkMock.Verify(
            x => x.SaveAllAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }
}