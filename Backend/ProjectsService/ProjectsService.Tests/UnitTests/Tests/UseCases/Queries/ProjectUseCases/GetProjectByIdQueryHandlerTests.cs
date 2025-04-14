using System.Linq.Expressions;
using ProjectsService.Application.Exceptions;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.ProjectUseCases;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetProjectByIdQueryHandler>> _loggerMock;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetProjectByIdQueryHandler>>();
        _handler = new GetProjectByIdQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository)
            .Returns(new Mock<IQueriesRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_WhenProjectExists_ReturnsProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetProjectByIdQuery(projectId);

        var project = new Project
        {
            Id = projectId,
            Title = "Test Project",
            Description = "Description",
            Budget = 1000,
            EmployerId = Guid.NewGuid(),
            Lifecycle = new Lifecycle
            {
                Status = ProjectStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            },
            Category = new Category
            {
                Name = "Development",
                NormalizedName = "DEVELOPMENT"
            },
            FreelancerApplications = new List<FreelancerApplication>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    FreelancerId = Guid.NewGuid(),
                    Status = ApplicationStatus.Pending
                }
            }
        };

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(
            projectId, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(project);
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByIdAsync(
            projectId, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Getting project by ID: {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Successfully retrieved project with ID: {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetProjectByIdQuery(projectId);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByIdAsync(
            projectId, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync((Project?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Project with ID '{projectId}' not found");

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByIdAsync(
            projectId, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Getting project by ID: {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning,
            $"Project with ID {projectId} not found", Times.Once());
    }
}