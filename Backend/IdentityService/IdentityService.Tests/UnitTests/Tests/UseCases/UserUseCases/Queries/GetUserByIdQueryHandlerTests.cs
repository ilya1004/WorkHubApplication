using System.Linq.Expressions;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Queries;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetUserByIdQueryHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new GetUserByIdQueryHandler(unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);
        var user = new AppUser
        {
            Id = userId,
            UserName = "user",
            Email = "user@example.com",
            FreelancerProfile = new FreelancerProfile { FirstName = "John", LastName = "Doe" },
            EmployerProfile = null,
            Role = new IdentityRole<Guid> { Name = "Freelancer" }
        };

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId,
            false,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(user);
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting user by ID: {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved user with ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId,
            false,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found");
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID '{userId}' not found", Times.Once());
    }
}