using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.ProjectUseCases;

public class GetProjectsByFilterQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetProjectsByFilterQueryHandler>> _loggerMock;
    private readonly GetProjectsByFilterQueryHandler _handler;

    public GetProjectsByFilterQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetProjectsByFilterQueryHandler>>();
        _handler = new GetProjectsByFilterQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository)
            .Returns(new Mock<IQueriesRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaginatedResult()
    {
        // Arrange
        var query = new GetProjectsByFilterQuery(
            Title: "Test Project",
            BudgetFrom: 1000,
            BudgetTo: 5000,
            CategoryId: Guid.NewGuid(),
            EmployerId: Guid.NewGuid(),
            ProjectStatus: ProjectStatus.InProgress,
            PageNo: 1,
            PageSize: 10);

        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Test Project 1", Budget = 2000, Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress } },
            new() { Id = Guid.NewGuid(), Title = "Test Project 2", Budget = 3000, Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress } }
        };
        var totalCount = 25;

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(projects);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Retrieved {projects.Count} filtered projects out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetProjectsByFilterQuery(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 2,
            PageSize: 5);

        var projects = new List<Project>();
        var totalCount = 0;

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.GetByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Retrieved {projects.Count} filtered projects out of {totalCount}", Times.Once());
    }
}