using System.Linq.Expressions;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserInfoById;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Queries;

public class GetEmployerUserInfoByIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetEmployerUserInfoByIdQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GetEmployerUserInfoByIdQueryHandler _handler;

    public GetEmployerUserInfoByIdQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetEmployerUserInfoByIdQueryHandler>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new GetEmployerUserInfoByIdQueryHandler(
            _loggerMock.Object,
            unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmployerUserDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetEmployerUserInfoByIdQuery(userId);
        var user = new AppUser
        {
            Id = userId,
            UserName = "employer",
            Email = "employer@example.com",
            EmployerProfile = new EmployerProfile
            {
                CompanyName = "Tech Corp",
                UserId = userId,
                Industry = new EmployerIndustry { Id = Guid.NewGuid(), Name = "Tech" }
            },
            Role = new IdentityRole<Guid> { Name = "Employer" }
        };
        var employerUserDto = new EmployerUserDto
        {
            Id = userId.ToString(),
            UserName = user.UserName,
            CompanyName = user.EmployerProfile.CompanyName,
            Email = user.Email,
            RoleName = user.Role.Name,
            Industry = new EmployerIndustryDto(user.EmployerProfile.Industry.Id.ToString(), user.EmployerProfile.Industry.Name)
        };

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId,
            false,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<EmployerUserDto>(user)).Returns(employerUserDto);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(employerUserDto);
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting employer info for user ID: {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved employer info for user ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetEmployerUserInfoByIdQuery(userId);

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
        _mapperMock.Verify(m => m.Map<EmployerUserDto>(It.IsAny<AppUser>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID '{userId}' not found", Times.Once());
    }
}