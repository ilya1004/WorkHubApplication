using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.ProjectUseCases;

public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<UpdateProjectCommandHandler>> _loggerMock;
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<UpdateProjectCommandHandler>>();
        _handler = new UpdateProjectCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository).Returns(new Mock<ICommandsRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.LifecycleQueriesRepository).Returns(new Mock<IQueriesRepository<Lifecycle>>().Object);
        _unitOfWorkMock.Setup(u => u.LifecycleCommandsRepository).Returns(new Mock<ICommandsRepository<Lifecycle>>().Object);
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
    }

    [Fact]
    public async Task Handle_WhenProjectAndLifecycleExistAndUserIsEmployerAndStatusIsPublished_UpdatesSuccessfully_WithCategory()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId,
            new UpdateProjectDto("Updated Title", null, 100m, categoryId),
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(8), DateTime.UtcNow.AddDays(14))
        );
        var project = new Project { Id = projectId, EmployerId = userId, Title = "Original Title" };
        var lifecycle = new Lifecycle { ProjectId = projectId, Status = ProjectStatus.Published };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.LifecycleQueriesRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Lifecycle, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lifecycle);
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.AnyAsync(
            It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map(command.Project, project)).Callback<UpdateProjectDto, Project>((dto, p) => p.Title = dto.Title);
        _mapperMock.Setup(m => m.Map(command.Lifecycle, lifecycle)).Callback<LifecycleDto, Lifecycle>((dto, l) =>
        {
            l.ApplicationsStartDate = dto.ApplicationsStartDate;
            l.ApplicationsDeadline = dto.ApplicationsDeadline;
            l.WorkStartDate = dto.WorkStartDate;
            l.WorkDeadline = dto.WorkDeadline;
        });
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.LifecycleCommandsRepository.UpdateAsync(lifecycle, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Title.Should().Be("Updated Title");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.UpdateAsync(lifecycle, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Updating project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Checking category {categoryId} existence", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Saving project {projectId} updates", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId, 
            new UpdateProjectDto("Updated Title", null, 100m, Guid.NewGuid()), 
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow));

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Project with ID '{projectId}' not found");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Project {projectId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotEmployer_ThrowsForbiddenException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId,
            new UpdateProjectDto("Updated Title", null, 100m, Guid.NewGuid()),
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow));
        var project = new Project { Id = projectId, EmployerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>().WithMessage($"You do not have access to project with ID '{projectId}'");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User {userId} attempted to update project {projectId} without permission", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenLifecycleNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId, 
            new UpdateProjectDto("Updated Title", null, 100m, Guid.NewGuid()), 
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow));
        var project = new Project { Id = projectId, EmployerId = userId };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.LifecycleQueriesRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Lifecycle, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lifecycle?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Project lifecycle with ProjectId '{projectId}' not found");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Lifecycle not found for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectStatusNotPublished_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId, 
            new UpdateProjectDto("Updated Title", null, 100m, Guid.NewGuid()), 
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow));
        var project = new Project { Id = projectId, EmployerId = userId };
        var lifecycle = new Lifecycle { ProjectId = projectId, Status = ProjectStatus.InProgress };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.LifecycleQueriesRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Lifecycle, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lifecycle);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("You cannot edit this project after the start of accepting applications");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid project status {lifecycle.Status} for update", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenCategoryIdProvidedAndNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId,
            new UpdateProjectDto("Updated Title", null, 100m, categoryId),
            new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow)
        );
        var project = new Project { Id = projectId, EmployerId = userId };
        var lifecycle = new Lifecycle { ProjectId = projectId, Status = ProjectStatus.Published };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _unitOfWorkMock.Setup(u => u.LifecycleQueriesRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Lifecycle, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(lifecycle);
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.AnyAsync(
            It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Category with ID '{categoryId}' not found");
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.UpdateAsync(It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Category {categoryId} not found", Times.Once());
    }
}