using System.Linq.Expressions;
using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Queries;

public class GetCurrentFreelancerUserQueryHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetCurrentFreelancerUserQueryHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GetCurrentFreelancerUserQueryHandler _handler;

    public GetCurrentFreelancerUserQueryHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _userContextMock = new Mock<IUserContext>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetCurrentFreelancerUserQueryHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new GetCurrentFreelancerUserQueryHandler(
            unitOfWorkMock.Object,
            _userContextMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFreelancerUserDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCurrentFreelancerUserQuery();
        var user = new AppUser
        {
            Id = userId,
            UserName = "freelancer",
            Email = "freelancer@example.com",
            FreelancerProfile = new FreelancerProfile
            {
                FirstName = "John",
                LastName = "Doe",
                UserId = userId,
                Skills = new List<FreelancerSkill>()
            },
            Role = new IdentityRole<Guid> { Name = "Freelancer" }
        };
        var freelancerUserDto = new FreelancerUserDto
        {
            Id = userId.ToString(),
            UserName = user.UserName,
            FirstName = user.FreelancerProfile.FirstName,
            LastName = user.FreelancerProfile.LastName,
            Email = user.Email,
            RoleName = user.Role.Name
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId,
            false,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<FreelancerUserDto>(user)).Returns(freelancerUserDto);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(freelancerUserDto);
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting current user info for user ID: {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved current user info for user ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCurrentFreelancerUserQuery();

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
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
        _mapperMock.Verify(m => m.Map<FreelancerUserDto>(It.IsAny<AppUser>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID '{userId}' not found", Times.Once());
    }
}