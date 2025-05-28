using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.UseCases.Queries.FreelancerApplicationUseCases;

public class GetFreelancerApplicationsByFilterQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetFreelancerApplicationsByFilterQueryHandler>> _loggerMock;
    private readonly GetFreelancerApplicationsByFilterQueryHandler _handler;

    public GetFreelancerApplicationsByFilterQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetFreelancerApplicationsByFilterQueryHandler>>();
        _handler = new GetFreelancerApplicationsByFilterQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository)
            .Returns(new Mock<IQueriesRepository<FreelancerApplication>>().Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaginatedResult()
    {
        // Arrange
        var query = new GetFreelancerApplicationsByFilterQuery(
            StartDate: DateTime.UtcNow.AddDays(-10),
            EndDate: DateTime.UtcNow,
            ApplicationStatus: ApplicationStatus.Pending,
            PageNo: 1,
            PageSize: 10);

        var applications = new List<FreelancerApplication>
        {
            new() { Id = Guid.NewGuid(), Status = ApplicationStatus.Pending },
            new() { Id = Guid.NewGuid(), Status = ApplicationStatus.Pending }
        };
        var totalCount = 50;

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(applications);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
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
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} filtered applications out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetFreelancerApplicationsByFilterQuery(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
            PageNo: 2,
            PageSize: 5);

        var applications = new List<FreelancerApplication>();
        var totalCount = 0;

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(applications);

        _unitOfWorkMock.Setup(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()))
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
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            It.IsAny<GetFreelancerApplicationsByFilterSpecification>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {applications.Count} filtered applications out of {totalCount}", Times.Once());
    }
}