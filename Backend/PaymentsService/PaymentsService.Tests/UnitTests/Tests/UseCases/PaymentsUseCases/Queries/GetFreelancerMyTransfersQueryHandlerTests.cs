using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Queries;

public class GetFreelancerMyTransfersQueryHandlerTests
{
    private readonly Mock<ITransfersService> _transfersServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetFreelancerMyTransfersQueryHandler>> _loggerMock;
    private readonly GetFreelancerMyTransfersQueryHandler _handler;

    public GetFreelancerMyTransfersQueryHandlerTests()
    {
        _transfersServiceMock = new Mock<ITransfersService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetFreelancerMyTransfersQueryHandler>>();
        _handler = new GetFreelancerMyTransfersQueryHandler(_transfersServiceMock.Object, _userContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithProjectId_ReturnsPaginatedTransfers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetFreelancerMyTransfersQuery(ProjectId: projectId, PageNo: 2, PageSize: 2);
        var transfers = new List<TransferModel>
        {
            new() { Id = "tr_1", Amount = 500, Currency = "EUR", TransferGroup = "group_1", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_2", Amount = 1000, Currency = "EUR", TransferGroup = "group_2", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_3", Amount = 1500, Currency = "EUR", TransferGroup = "group_3", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_4", Amount = 2000, Currency = "EUR", TransferGroup = "group_4", Metadata = new Dictionary<string, string>() }
        };
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetFreelancerTransfersAsync(userId, projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(transfers.Skip(2).Take(2));
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetFreelancerTransfersAsync(userId, projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving transfers for freelancer {userId}, project {projectId}, page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} transfers for freelancer {userId}, project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WithoutProjectId_ReturnsPaginatedTransfers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetFreelancerMyTransfersQuery(ProjectId: null, PageNo: 1, PageSize: 3);
        var transfers = new List<TransferModel>
        {
            new() { Id = "tr_1", Amount = 500, Currency = "EUR", TransferGroup = "group_1", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_2", Amount = 1000, Currency = "EUR", TransferGroup = "group_2", Metadata = new Dictionary<string, string>() }
        };
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetFreelancerTransfersAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(transfers);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetFreelancerTransfersAsync(userId, null, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetFreelancerMyTransfersQuery(ProjectId: projectId, PageNo: 1, PageSize: 10);
        var transfers = new List<TransferModel>();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetFreelancerTransfersAsync(userId, projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _transfersServiceMock.Verify(s => s.GetFreelancerTransfersAsync(userId, projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving transfers for freelancer {userId}, project {projectId}, page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 transfers for freelancer {userId}, project {projectId}", Times.Once());
    }
}