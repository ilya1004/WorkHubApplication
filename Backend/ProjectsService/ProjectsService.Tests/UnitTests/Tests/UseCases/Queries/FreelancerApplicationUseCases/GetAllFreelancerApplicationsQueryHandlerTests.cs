using System.Linq.Expressions;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.FreelancerApplicationUseCases;

public class GetAllFreelancerApplicationsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetAllFreelancerApplicationsQueryHandler>> _loggerMock;
    private readonly GetAllFreelancerApplicationsQueryHandler _handler;

    public GetAllFreelancerApplicationsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetAllFreelancerApplicationsQueryHandler>>();
        _handler = new GetAllFreelancerApplicationsQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository)
            .Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_WhenApplicationsExist_ReturnsPaginatedResult()
    {
        // Arrange
        var pageNo = 2;
        var pageSize = 3;
        var query = new GetAllFreelancerApplicationsQuery(pageNo, pageSize);
        var applications = new List<FreelancerApplication>
        {
            new FreelancerApplication
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = ApplicationStatus.Accepted,
                ProjectId = Guid.NewGuid(),
                FreelancerId = Guid.NewGuid(),
                Project = new Project { Id = Guid.NewGuid() }
            },
            new FreelancerApplication
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = ApplicationStatus.Accepted,
                ProjectId = Guid.NewGuid(),
                FreelancerId = Guid.NewGuid(),
                Project = new Project { Id = Guid.NewGuid() }
            },
            new FreelancerApplication
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = ApplicationStatus.Rejected,
                ProjectId = Guid.NewGuid(),
                FreelancerId = Guid.NewGuid(),
                Project = new Project { Id = Guid.NewGuid() }
            }
        };
        var totalCount = 10;
        var offset = (pageNo - 1) * pageSize;

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.PaginatedListAllAsync(
            offset, pageSize, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()))
            .ReturnsAsync(applications);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(applications);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(pageNo);
        result.PageSize.Should().Be(pageSize);
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.PaginatedListAllAsync(
            offset, pageSize, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting all freelancer applications with pagination - Page: {pageNo}, Size: {pageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} applications out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenNoApplicationsExist_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var pageNo = 1;
        var pageSize = 5;
        var query = new GetAllFreelancerApplicationsQuery(pageNo, pageSize);
        var applications = new List<FreelancerApplication>();
        var totalCount = 0;
        var offset = (pageNo - 1) * pageSize;

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.PaginatedListAllAsync(
            offset, pageSize, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()))
            .ReturnsAsync(applications);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(pageNo);
        result.PageSize.Should().Be(pageSize);
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.PaginatedListAllAsync(
            offset, pageSize, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<FreelancerApplication, object>>[]>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting all freelancer applications with pagination - Page: {pageNo}, Size: {pageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} applications out of {totalCount}", Times.Once());
    }
}