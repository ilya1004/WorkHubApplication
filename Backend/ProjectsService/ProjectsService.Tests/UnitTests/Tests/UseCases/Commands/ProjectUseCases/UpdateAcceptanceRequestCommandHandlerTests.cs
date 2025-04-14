using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.ProjectUseCases;

public class UpdateAcceptanceRequestCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<UpdateAcceptanceRequestCommandHandler>> _loggerMock;
    private readonly UpdateAcceptanceRequestCommandHandler _handler;

    public UpdateAcceptanceRequestCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<UpdateAcceptanceRequestCommandHandler>>();
        _handler = new UpdateAcceptanceRequestCommandHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository).Returns(new Mock<ICommandsRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_WhenProjectExistsAndUserIsFreelancerAndStatusIsValid_UpdatesAcceptanceRequestSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceRequestCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            FreelancerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress }
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
        project.Lifecycle.AcceptanceRequested.Should().BeTrue();
        project.Lifecycle.Status.Should().Be(ProjectStatus.PendingForReview);
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Updating acceptance request for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Setting acceptance requested for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated acceptance request for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new UpdateAcceptanceRequestCommand(projectId);

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
    public async Task Handle_WhenUserNotFreelancer_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceRequestCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            FreelancerId = Guid.NewGuid(),
            Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress }
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
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to update acceptance for project {projectId} without permission", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectStatusInvalid_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateAcceptanceRequestCommand(projectId);
        var project = new Project
        {
            Id = projectId,
            FreelancerId = userId,
            Lifecycle = new Lifecycle { Status = ProjectStatus.Published }
        };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Current project status do not allow you to send acceptance request");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid project status {project.Lifecycle.Status} for acceptance request", Times.Once());
    }
}