using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.ProjectUseCases;

public class GetProjectsByEmployerFilterQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetProjectsByEmployerFilterQueryHandler>> _loggerMock;
    private readonly GetProjectsByEmployerFilterQueryHandler _handler;

    public GetProjectsByEmployerFilterQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetProjectsByEmployerFilterQueryHandler>>();
        _handler = new GetProjectsByEmployerFilterQueryHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository)
            .Returns(new Mock<IQueriesRepository<Project>>().Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetProjectsByEmployerFilterQuery(
            UpdatedAtStartDate: DateTime.UtcNow.AddDays(-10),
            UpdatedAtEndDate: DateTime.UtcNow,
            ProjectStatus: ProjectStatus.InProgress,
            AcceptanceRequestedAndNotConfirmed: true,
            PageNo: 1,
            PageSize: 10);

        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1", EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress } },
            new() { Id = Guid.NewGuid(), Title = "Project 2", EmployerId = userId, Lifecycle = new Lifecycle { Status = ProjectStatus.InProgress } }
        };
        var totalCount = 50;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByFilterAsync(
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()))
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
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Retrieved {projects.Count} employer filtered projects out of {totalCount} for user {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetProjectsByEmployerFilterQuery(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 2,
            PageSize: 5);

        var projects = new List<Project>();
        var totalCount = 0;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.GetByFilterAsync(
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);
        _unitOfWorkMock.Setup(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()))
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
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectQueriesRepository.CountByFilterAsync(
            It.IsAny<GetProjectsByEmployerFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information,
            $"Retrieved {projects.Count} employer filtered projects out of {totalCount} for user {userId}", Times.Once());
    }
}