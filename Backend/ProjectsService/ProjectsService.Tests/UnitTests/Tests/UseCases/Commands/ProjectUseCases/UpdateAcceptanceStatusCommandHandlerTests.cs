using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.ProjectUseCases;

public class UpdateAcceptanceStatusCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<UpdateAcceptanceStatusCommandHandler>> _loggerMock;
    private readonly UpdateAcceptanceStatusCommandHandler _handler;

    public UpdateAcceptanceStatusCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<UpdateAcceptanceStatusCommandHandler>>();
        _handler = new UpdateAcceptanceStatusCommandHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository).Returns(new Mock<ICommandsRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsEmployerAndAcceptanceRequested_ConfirmsAcceptanceSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceStatusCommand(projectId, true);
        var project = new Project
        {
            Id = projectId,
            EmployerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.PendingForReview, AcceptanceRequested = true }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Lifecycle.AcceptanceConfirmed.Should().BeTrue();
        project.Lifecycle.Status.Should().Be(ProjectStatus.Completed);
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Updating acceptance status for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Confirming acceptance for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated acceptance status for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsEmployerAndAcceptanceRequested_RejectsAcceptanceSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceStatusCommand(projectId, false);
        var project = new Project
        {
            Id = projectId,
            EmployerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.PendingForReview, AcceptanceRequested = true }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Lifecycle.AcceptanceRequested.Should().BeFalse();
        project.Lifecycle.AcceptanceConfirmed.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Updating acceptance status for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Rejecting acceptance for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated acceptance status for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new UpdateAcceptanceStatusCommand(projectId, true);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Project with ID '{projectId}' not found");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project {projectId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotEmployer_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceStatusCommand(projectId, true);
        var project = new Project
        {
            Id = projectId,
            EmployerId = Guid.NewGuid(),
            Lifecycle = new Lifecycle { Status = ProjectStatus.PendingForReview, AcceptanceRequested = true }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to project with ID '{projectId}'");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to update acceptance status for project {projectId} without permission", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenAcceptanceNotRequested_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceStatusCommand(projectId, true);
        var project = new Project
        {
            Id = projectId,
            EmployerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.PendingForReview, AcceptanceRequested = false }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Project acceptance is not requested yet");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Acceptance not requested for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectStatusInvalid_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceStatusCommand(projectId, true);
        var project = new Project
        {
            Id = projectId,
            EmployerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress, AcceptanceRequested = true }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Current project status do not allow you to update acceptance status");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid project status {project.Lifecycle.Status} for acceptance update", Times.Once());
    }
}