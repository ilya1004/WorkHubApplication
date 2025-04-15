using System.Linq.Expressions;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Queries;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<ILogger<GetAllUsersQueryHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetAllUsersQueryHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new GetAllUsersQueryHandler(unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedUsers_WhenDataExists()
    {
        // Arrange
        var pageNo = 2;
        var pageSize = 5;
        var query = new GetAllUsersQuery(pageNo, pageSize);
        var users = new List<AppUser>
        {
            new AppUser { Id = Guid.NewGuid(), UserName = "user1", Email = "user1@example.com" },
            new AppUser { Id = Guid.NewGuid(), UserName = "user2", Email = "user2@example.com" }
        };
        var totalCount = 12;

        _usersRepositoryMock.Setup(r => r.PaginatedListAllAsync(
            (pageNo - 1) * pageSize,
            pageSize,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(users);
        _usersRepositoryMock.Setup(r => r.CountAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(totalCount);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new PaginatedResultModel<AppUser>
        {
            Items = users,
            TotalCount = totalCount,
            PageNo = pageNo,
            PageSize = pageSize
        });
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting all users with pagination - PageNo: {pageNo}, PageSize: {pageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {users.Count} users from repository", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Total users count: {totalCount}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPaginatedResult_WhenNoData()
    {
        // Arrange
        var pageNo = 1;
        var pageSize = 10;
        var query = new GetAllUsersQuery(pageNo, pageSize);
        var users = new List<AppUser>();
        var totalCount = 0;

        _usersRepositoryMock.Setup(r => r.PaginatedListAllAsync(
            0,
            pageSize,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(users);
        _usersRepositoryMock.Setup(r => r.CountAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(totalCount);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(new PaginatedResultModel<AppUser>
        {
            Items = users,
            TotalCount = totalCount,
            PageNo = pageNo,
            PageSize = pageSize
        });
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting all users with pagination - PageNo: {pageNo}, PageSize: {pageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {users.Count} users from repository", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Total users count: {totalCount}", Times.Once());
    }
}