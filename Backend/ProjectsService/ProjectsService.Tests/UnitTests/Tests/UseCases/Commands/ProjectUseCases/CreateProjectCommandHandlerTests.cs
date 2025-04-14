using System.Linq.Expressions;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Commands.ProjectUseCases;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<CreateProjectCommandHandler>> _loggerMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<CreateProjectCommandHandler>>();
        _handler = new CreateProjectCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository).Returns(new Mock<ICommandsRepository<Project>>().Object);
    }

    [Fact]
public async Task Handle_WhenValidCommand_CreatesProjectSuccessfully()
{
    // Arrange
    var projectId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var categoryId = Guid.NewGuid();
    var command = new CreateProjectCommand(
        new ProjectDto("Test Project", "Description", 1000m, categoryId),
        new LifecycleDto(DateTime.UtcNow, DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(8), DateTime.UtcNow.AddDays(14))
    );
    var project = new Project
    {
        Id = projectId,
        Title = command.Project.Title,
        Description = command.Project.Description!,
        Budget = command.Project.Budget,
        CategoryId = command.Project.CategoryId,
        EmployerId = userId
    };
    var lifecycle = new Lifecycle
    {
        ProjectId = projectId,
        ApplicationsStartDate = command.Lifecycle.ApplicationsStartDate,
        ApplicationsDeadline = command.Lifecycle.ApplicationsDeadline,
        WorkStartDate = command.Lifecycle.WorkStartDate,
        WorkDeadline = command.Lifecycle.WorkDeadline
    };

    _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
    _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository).Returns(new Mock<IQueriesRepository<Project>>().Object);
    _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.FirstOrDefaultAsync(
        It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Project?)null);
    _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository).Returns(new Mock<IQueriesRepository<Category>>().Object);
    _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.AnyAsync(
            It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    _mapperMock.Setup(m => m.Map<Project>(command.Project)).Returns(project);
    _mapperMock.Setup(m => m.Map<Lifecycle>(command)).Returns(lifecycle);
    _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository.AddAsync(project, It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);
    _unitOfWorkMock.Setup(u => u.LifecycleCommandsRepository).Returns(new Mock<ICommandsRepository<Lifecycle>>().Object);
    _unitOfWorkMock.Setup(u => u.LifecycleCommandsRepository.AddAsync(lifecycle, It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);
    _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().Be(projectId);
    project.EmployerId.Should().Be(userId);
    _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.FirstOrDefaultAsync(
        It.IsAny<Expression<Func<Project, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
    _unitOfWorkMock.Verify(u => u.CategoryQueriesRepository.AnyAsync(
        It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()), Times.Once());
    _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.AddAsync(project, It.IsAny<CancellationToken>()), Times.Once());
    _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.AddAsync(lifecycle, It.IsAny<CancellationToken>()), Times.Once());
    _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
    _loggerMock.VerifyLog(LogLevel.Information, $"User {userId} creating new project with title '{command.Project.Title}'", Times.Once());
    _loggerMock.VerifyLog(LogLevel.Information, $"Checking category {categoryId} existence", Times.Once());
    _loggerMock.VerifyLog(LogLevel.Information, $"Adding new project {projectId}", Times.Once());
    _loggerMock.VerifyLog(LogLevel.Information, $"Adding lifecycle for project {projectId}", Times.Once());
    _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created project {projectId}", Times.Once());
}
}