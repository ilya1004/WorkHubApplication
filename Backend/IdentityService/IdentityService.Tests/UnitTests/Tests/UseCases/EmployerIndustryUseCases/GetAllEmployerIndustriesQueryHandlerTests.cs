using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.EmployerIndustryUseCases;

public class GetAllEmployerIndustriesQueryHandlerTests
{
    private readonly Mock<ILogger<GetAllEmployerIndustriesQueryHandler>> _loggerMock;
    private readonly Mock<IRepository<EmployerIndustry>> _industriesRepositoryMock;
    private readonly GetAllEmployerIndustriesQueryHandler _handler;

    public GetAllEmployerIndustriesQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetAllEmployerIndustriesQueryHandler>>();
        _industriesRepositoryMock = new Mock<IRepository<EmployerIndustry>>();

        unitOfWorkMock.Setup(u => u.EmployerIndustriesRepository).Returns(_industriesRepositoryMock.Object);

        _handler = new GetAllEmployerIndustriesQueryHandler(unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedIndustries_WhenDataExists()
    {
        // Arrange
        var pageNo = 2;
        var pageSize = 5;
        var command = new GetAllEmployerIndustriesQuery(pageNo, pageSize);
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Tech" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Finance" }
        };
        var totalCount = 12;

        _industriesRepositoryMock.Setup(r => r.PaginatedListAllAsync((pageNo - 1) * pageSize, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(industries);
        _industriesRepositoryMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(totalCount);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new PaginatedResultModel<EmployerIndustry>
        {
            Items = industries,
            TotalCount = totalCount,
            PageNo = pageNo,
            PageSize = pageSize
        });
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {industries.Count} industries out of {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPaginatedResult_WhenNoData()
    {
        // Arrange
        var pageNo = 1;
        var pageSize = 10;
        var command = new GetAllEmployerIndustriesQuery(pageNo, pageSize);
        var industries = new List<EmployerIndustry>();
        var totalCount = 0;

        _industriesRepositoryMock.Setup(r => r.PaginatedListAllAsync(0, pageSize, It.IsAny<CancellationToken>())).ReturnsAsync(industries);
        _industriesRepositoryMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(totalCount);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new PaginatedResultModel<EmployerIndustry>
        {
            Items = industries,
            TotalCount = totalCount,
            PageNo = pageNo,
            PageSize = pageSize
        });
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {industries.Count} industries out of {totalCount}", Times.Once());
    }
}