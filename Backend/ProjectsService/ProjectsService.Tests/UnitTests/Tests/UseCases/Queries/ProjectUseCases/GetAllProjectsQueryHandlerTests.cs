using System.Linq.Expressions;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.ProjectUseCases;

public class GetAllProjectsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetAllProjectsQueryHandler>> _loggerMock;
    private readonly GetAllProjectsQueryHandler _handler;

    public GetAllProjectsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetAllProjectsQueryHandler>>();
        _handler = new GetAllProjectsQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository)
            .Returns(new Mock<IQueriesRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaginatedResult()
    {
        // Arrange
        var query = new GetAllProjectsQuery(PageNo: 1, PageSize: 10);

        var projects = new List<Project>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Project 1",
                Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress },
                Category = new Category { Name = "Development" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Project 2",
                Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress },
                Category = new Category { Name = "Design" }
            }
        };
        var totalCount = 50;

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.PaginatedListAllAsync(
            0, query.PageSize, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(projects);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.PaginatedListAllAsync(
            0, query.PageSize, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Getting all projects with pagination - Page: {query.PageNo}, Size: {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Retrieved {projects.Count} projects out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetAllProjectsQuery(PageNo: 2, PageSize: 5);

        var projects = new List<Project>();
        var totalCount = 0;

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.PaginatedListAllAsync(
            5, query.PageSize, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(projects);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.PaginatedListAllAsync(
            5, query.PageSize, It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Project, object>>[]>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Retrieved {projects.Count} projects out of {totalCount}", Times.Once());
    }
}