using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.FreelancerApplicationUseCases;

public class GetMyFreelancerApplicationsByFilterQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetMyFreelancerApplicationsByFilterQueryHandler>> _loggerMock; 
    private readonly GetMyFreelancerApplicationsByFilterQueryHandler _handler;

    public GetMyFreelancerApplicationsByFilterQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetMyFreelancerApplicationsByFilterQueryHandler>>();
        _handler = new GetMyFreelancerApplicationsByFilterQueryHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository)
            .Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMyFreelancerApplicationsByFilterQuery(
            StartDate: DateTime.UtcNow.AddDays(-5),
            EndDate: DateTime.UtcNow,
            ApplicationStatus: ApplicationStatus.Accepted,
            PageNo: 1,
            PageSize: 20);

        var applications = new List<FreelancerApplication>
        {
            new() { Id = Guid.NewGuid(), FreelancerId = userId, Status = ApplicationStatus.Accepted },
            new() { Id = Guid.NewGuid(), FreelancerId = userId, Status = ApplicationStatus.Accepted }
        };
        var totalCount = 30;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(applications);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(applications);
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} filtered applications out of {totalCount} for user {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMyFreelancerApplicationsByFilterQuery(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
            PageNo: 3,
            PageSize: 15);

        var applications = new List<FreelancerApplication>();
        var totalCount = 0;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(applications);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(totalCount);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetMyFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} filtered applications out of {totalCount} for user {userId}", Times.Once());
    }
}