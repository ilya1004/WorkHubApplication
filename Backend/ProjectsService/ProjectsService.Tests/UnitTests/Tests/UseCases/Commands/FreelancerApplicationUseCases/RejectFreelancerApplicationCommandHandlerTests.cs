using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.FreelancerApplicationUseCases;

public class RejectFreelancerApplicationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<RejectFreelancerApplicationCommandHandler>> _loggerMock;
    private readonly RejectFreelancerApplicationCommandHandler _handler;

    public RejectFreelancerApplicationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<RejectFreelancerApplicationCommandHandler>>();
        _handler = new RejectFreelancerApplicationCommandHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository).Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository).Returns(new Mock<ICommandsRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_WhenValidConditions_RejectsApplicationSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new RejectFreelancerApplicationCommand(projectId, applicationId);
        var project = new Project { Id = projectId, EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications } };
        var application = new FreelancerApplication { Id = applicationId, Status = ApplicationStatus.Accepted };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(application);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository.UpdateAsync(It.IsAny<FreelancerApplication>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        application.Status.Should().Be(ApplicationStatus.Pending);
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationCommandsRepository.UpdateAsync(application, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Rejecting freelancer application {applicationId} for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully rejected freelancer application {applicationId} for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var command = new RejectFreelancerApplicationCommand(projectId, applicationId);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Project with ID '{projectId}' not found");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project {projectId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotEmployer_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new RejectFreelancerApplicationCommand(projectId, applicationId);
        var project = new Project { Id = projectId, EmployerId = Guid.NewGuid(), Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications } };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to project with ID '{projectId}'");
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to access project {projectId} without permission", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenInvalidProjectStatus_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new RejectFreelancerApplicationCommand(projectId, applicationId);
        var project = new Project { Id = projectId, EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress } };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("You can reject applications to this project only during accepting applications stage");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid project status {project.Lifecycle.Status} for rejecting applications", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenApplicationNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new RejectFreelancerApplicationCommand(projectId, applicationId);
        var project = new Project { Id = projectId, EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications } };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FreelancerApplication?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Freelancer application with ID '{applicationId}' not found");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer application {applicationId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenApplicationStatusNotAccepted_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var applicationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new RejectFreelancerApplicationCommand(projectId, applicationId);
        var project = new Project { Id = projectId, EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.AcceptingApplications } };
        var application = new FreelancerApplication { Id = applicationId, Status = ApplicationStatus.Pending };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByIdAsync(applicationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(application);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Freelancer application status is not accepted");
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer application {applicationId} has invalid status {application.Status}", Times.Once());
    }
}