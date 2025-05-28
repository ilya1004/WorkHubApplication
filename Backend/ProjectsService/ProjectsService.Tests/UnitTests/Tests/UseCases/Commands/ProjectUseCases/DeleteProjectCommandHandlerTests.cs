using System.Linq.Expressions;
using ProjectsService.Application.Constants;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.ProjectUseCases;

public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<DeleteProjectCommandHandler>> _loggerMock;
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<DeleteProjectCommandHandler>>();
        _handler = new DeleteProjectCommandHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository).Returns(new Mock<ICommandsRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsEmployerAndStatusIsCancelled_DeletesProjectSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteProjectCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            EmployerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.Cancelled }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.DeleteAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.DeleteAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting project {projectId}", Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsAdminAndStatusIsCancelled_DeletesProjectSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteProjectCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            EmployerId = Guid.NewGuid(),
            Lifecycle = new Lifecycle { Status = ProjectStatus.Cancelled }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.AdminRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.DeleteAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.DeleteAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting project {projectId}", Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new DeleteProjectCommand(projectId);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Project with ID '{projectId}' not found");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.DeleteAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project {projectId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotEmployerOrAdmin_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteProjectCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            EmployerId = Guid.NewGuid(),
            Lifecycle = new Lifecycle { Status = ProjectStatus.Cancelled }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to project with ID '{projectId}'");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.DeleteAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to delete project {projectId} without permission", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectStatusNotCancelled_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteProjectCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            EmployerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _userContextMock.Setup(u => u.GetUserRole()).Returns(AppRoles.FreelancerRole);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("You need to cancel the project before its removing");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.DeleteAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project {projectId} must be cancelled before deletion", Times.Once());
    }
}