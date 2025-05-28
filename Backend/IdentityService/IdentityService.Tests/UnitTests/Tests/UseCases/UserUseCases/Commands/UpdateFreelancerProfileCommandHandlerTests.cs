using System.Linq.Expressions;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Commands;

public class UpdateFreelancerProfileCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<UpdateFreelancerProfileCommandHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IRepository<FreelancerSkill>> _skillsRepositoryMock;
    private readonly UpdateFreelancerProfileCommandHandler _handler;

    public UpdateFreelancerProfileCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<UpdateFreelancerProfileCommandHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _skillsRepositoryMock = new Mock<IRepository<FreelancerSkill>>();

        _unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.FreelancerSkillsRepository).Returns(_skillsRepositoryMock.Object);

        _handler = new UpdateFreelancerProfileCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _blobServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenValidInputWithSkillsAndImage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var skillId1 = Guid.NewGuid();
        var skillId2 = Guid.NewGuid();
        var newImageId = Guid.NewGuid();
        var command = new UpdateFreelancerProfileCommand(
            new FreelancerProfileDto("John", "Doe", "About", new[] { skillId1, skillId2 }, false),
            new MemoryStream(),
            "image/jpeg");
        var user = new AppUser
        {
            Id = userId,
            FreelancerProfile = new FreelancerProfile
            {
                Id = Guid.NewGuid(),
                Skills = new List<FreelancerSkill> { new FreelancerSkill { Id = Guid.NewGuid(), Name = "Old Skill" } }
            },
            ImageUrl = Guid.NewGuid().ToString()
        };
        var newSkills = new List<FreelancerSkill>
        {
            new FreelancerSkill { Id = skillId1, Name = "Skill1" },
            new FreelancerSkill { Id = skillId2, Name = "Skill2" }
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.FreelancerProfile, user.FreelancerProfile)).Returns(user.FreelancerProfile);
        _skillsRepositoryMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<FreelancerSkill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newSkills);
        _blobServiceMock.Setup(b => b.DeleteAsync(Guid.Parse(user.ImageUrl), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _blobServiceMock.Setup(b => b.UploadAsync(command.FileStream!, command.ContentType!, It.IsAny<CancellationToken>())).ReturnsAsync(newImageId);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        user.FreelancerProfile.Skills.Should().BeEquivalentTo(newSkills);
        user.ImageUrl.Should().Be(newImageId.ToString());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated freelancer profile for user ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateFreelancerProfileCommand(
            new FreelancerProfileDto("John", "Doe", null, null, false),
            null,
            null);

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found");
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenInvalidContentType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateFreelancerProfileCommand(
            new FreelancerProfileDto("John", "Doe", null, null, false),
            new MemoryStream(),
            "text/plain");
        var user = new AppUser
        {
            Id = userId,
            FreelancerProfile = new FreelancerProfile
            {
                Id = Guid.NewGuid(),
                Skills = new List<FreelancerSkill>()
            }
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.FreelancerProfile, user.FreelancerProfile)).Returns(user.FreelancerProfile);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Only image files are allowed.");
        _blobServiceMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Invalid file type: {command.ContentType}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldResetImageAndClearSkills_WhenResetImageIsTrueAndNoSkills()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateFreelancerProfileCommand(
            new FreelancerProfileDto("John", "Doe", null, null, true),
            null,
            null);
        var user = new AppUser
        {
            Id = userId,
            FreelancerProfile = new FreelancerProfile
            {
                Id = Guid.NewGuid(),
                Skills = new List<FreelancerSkill> { new FreelancerSkill { Id = Guid.NewGuid(), Name = "Old Skill" } }
            },
            ImageUrl = Guid.NewGuid().ToString()
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.FreelancerProfile, user.FreelancerProfile)).Returns(user.FreelancerProfile);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        user.FreelancerProfile.Skills.Should().BeEmpty();
        user.ImageUrl.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated freelancer profile for user ID: {userId}", Times.Once());
    }
}