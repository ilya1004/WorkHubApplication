using System.Linq.Expressions;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserInfoById;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Queries;

public class GetFreelancerUserInfoByIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetFreelancerUserInfoByIdQueryHandler>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GetFreelancerUserInfoByIdQueryHandler _handler;

    public GetFreelancerUserInfoByIdQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetFreelancerUserInfoByIdQueryHandler>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);

        _handler = new GetFreelancerUserInfoByIdQueryHandler(
            _loggerMock.Object,
            unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFreelancerUserDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetFreelancerUserInfoByIdQuery(userId);
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
                Skills = new List<FreelancerSkill>
                {
                    new FreelancerSkill { Id = Guid.NewGuid(), Name = "C#" }
                }
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
            RoleName = user.Role.Name,
            Skills = new List<FreelancerSkillDto>
            {
                new FreelancerSkillDto(user.FreelancerProfile.Skills.First().Id.ToString(), user.FreelancerProfile.Skills.First().Name)
            }
        };

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
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer info for user ID: {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved freelancer info for user ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetFreelancerUserInfoByIdQuery(userId);

        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId,
            false,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync((AppUser)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found");
        _mapperMock.Verify(m => m.Map<FreelancerUserDto>(It.IsAny<AppUser>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID '{userId}' not found", Times.Once());
    }
}