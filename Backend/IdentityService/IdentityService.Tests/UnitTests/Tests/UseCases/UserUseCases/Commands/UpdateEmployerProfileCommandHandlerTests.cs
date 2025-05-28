using System.Linq.Expressions;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Commands;

public class UpdateEmployerProfileCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<UpdateEmployerProfileCommandHandler>> _loggerMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IRepository<EmployerIndustry>> _industriesRepositoryMock;
    private readonly UpdateEmployerProfileCommandHandler _handler;

    public UpdateEmployerProfileCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<UpdateEmployerProfileCommandHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _industriesRepositoryMock = new Mock<IRepository<EmployerIndustry>>();

        _unitOfWorkMock.Setup(u => u.UsersRepository).Returns(_usersRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EmployerIndustriesRepository).Returns(_industriesRepositoryMock.Object);

        _handler = new UpdateEmployerProfileCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _blobServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenValidInputWithNewIndustryAndImage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var industryId = Guid.NewGuid();
        var newImageId = Guid.NewGuid();
        var command = new UpdateEmployerProfileCommand(
            new EmployerProfileDto("New Corp", "About", industryId, false),
            new MemoryStream(),
            "image/jpeg");
        var user = new AppUser
        {
            Id = userId,
            EmployerProfile = new EmployerProfile { Id = Guid.NewGuid() },
            ImageUrl = Guid.NewGuid().ToString()
        };
        var industry = new EmployerIndustry { Id = industryId, Name = "Tech" };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.EmployerProfile, user.EmployerProfile)).Returns(user.EmployerProfile);
        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync(industry);
        _blobServiceMock.Setup(b => b.DeleteAsync(Guid.Parse(user.ImageUrl), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _blobServiceMock.Setup(b => b.UploadAsync(command.FileStream!, command.ContentType!, It.IsAny<CancellationToken>())).ReturnsAsync(newImageId);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        user.EmployerProfile.Industry.Should().Be(industry);
        user.ImageUrl.Should().Be(newImageId.ToString());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated employer profile for user ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateEmployerProfileCommand(
            new EmployerProfileDto("New Corp", null, null, false),
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
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID {userId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenIndustryNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var industryId = Guid.NewGuid();
        var command = new UpdateEmployerProfileCommand(
            new EmployerProfileDto("New Corp", null, industryId, false),
            null,
            null);
        var user = new AppUser
        {
            Id = userId,
            EmployerProfile = new EmployerProfile { Id = Guid.NewGuid() }
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.EmployerProfile, user.EmployerProfile)).Returns(user.EmployerProfile);
        _industriesRepositoryMock.Setup(r => r.GetByIdAsync(industryId, It.IsAny<CancellationToken>())).ReturnsAsync((EmployerIndustry)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Industry with ID '{industryId}' not found");
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Industry with ID {industryId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenInvalidContentType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateEmployerProfileCommand(
            new EmployerProfileDto("New Corp", null, null, false),
            new MemoryStream(),
            "text/plain");
        var user = new AppUser
        {
            Id = userId,
            EmployerProfile = new EmployerProfile { Id = Guid.NewGuid() }
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.EmployerProfile, user.EmployerProfile)).Returns(user.EmployerProfile);

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
    public async Task Handle_ShouldResetImage_WhenResetImageIsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateEmployerProfileCommand(
            new EmployerProfileDto("New Corp", null, null, true),
            null,
            null);
        var user = new AppUser
        {
            Id = userId,
            EmployerProfile = new EmployerProfile { Id = Guid.NewGuid() },
            ImageUrl = Guid.NewGuid().ToString()
        };

        _userContextMock.Setup(c => c.GetUserId()).Returns(userId);
        _usersRepositoryMock.Setup(r => r.GetByIdAsync(
            userId, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<AppUser, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map(command.EmployerProfile, user.EmployerProfile)).Returns(user.EmployerProfile);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        user.ImageUrl.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully updated employer profile for user ID: {userId}", Times.Once());
    }
}