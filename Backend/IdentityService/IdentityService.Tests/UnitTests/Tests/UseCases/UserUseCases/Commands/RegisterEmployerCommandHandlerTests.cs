using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Constants;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.UserUseCases.Commands;

public class RegisterEmployerCommandHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<ICachedService> _cachedServiceMock;
    private readonly Mock<ILogger<RegisterEmployerCommandHandler>> _loggerMock;
    private readonly Mock<IRepository<EmployerProfile>> _employerProfilesRepositoryMock;
    private readonly RegisterEmployerCommandHandler _handler;

    public RegisterEmployerCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
            Mock.Of<IRoleStore<IdentityRole<Guid>>>(), null!, null!, null!, null!);
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _emailSenderMock = new Mock<IEmailSender>();
        _cachedServiceMock = new Mock<ICachedService>();
        var configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<RegisterEmployerCommandHandler>>();
        _employerProfilesRepositoryMock = new Mock<IRepository<EmployerProfile>>();

        _unitOfWorkMock.Setup(u => u.EmployerProfilesRepository).Returns(_employerProfilesRepositoryMock.Object);

        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(s => s.Value).Returns("24");
        configurationMock.Setup(c => c.GetSection("IdentityTokenExpirationTimeInHours")).Returns(configSectionMock.Object);

        _handler = new RegisterEmployerCommandHandler(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _emailSenderMock.Object,
            _cachedServiceMock.Object,
            configurationMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRegisterEmployer_WhenValidInput()
    {
        // Arrange
        var command = new RegisterEmployerCommand("employer", "Tech Corp", "employer@example.com", "P@ssw0rd123");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        var employerProfile = new EmployerProfile { Id = Guid.NewGuid(), UserId = user.Id };
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = AppRoles.EmployerRole };
        var token = "confirmation-token";
        var code = "123456";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null!);
        _roleManagerMock.Setup(r => r.FindByNameAsync(AppRoles.EmployerRole)).ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map<AppUser>(command)).Returns(user);
        _userManagerMock.Setup(m => m.CreateAsync(user, command.Password)).ReturnsAsync(IdentityResult.Success);
        _mapperMock.Setup(m => m.Map<EmployerProfile>(command)).Returns(employerProfile);
        _employerProfilesRepositoryMock.Setup(r => r.AddAsync(employerProfile, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAllAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userManagerMock.Setup(m => m.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync(token);
        _cachedServiceMock.Setup(c => c.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _cachedServiceMock.Setup(c => c.SetAsync(code, token, TimeSpan.FromHours(24), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _emailSenderMock.Setup(e => e.SendEmailConfirmation(user.Email, code, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _userManagerMock.Verify(m => m.CreateAsync(user, command.Password), Times.Once());
        _employerProfilesRepositoryMock.Verify(r => r.AddAsync(employerProfile, It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _emailSenderMock.Verify(e => e.SendEmailConfirmation(user.Email, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully registered employer with ID: {user.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExists_WhenEmailExists()
    {
        // Arrange
        var command = new RegisterEmployerCommand("employer", "Tech Corp", "employer@example.com", "P@ssw0rd123");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage($"A user with the email '{command.Email}' already exists.");
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with email {command.Email} already exists", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenRoleNotFound()
    {
        // Arrange
        var command = new RegisterEmployerCommand("employer", "Tech Corp", "employer@example.com", "P@ssw0rd123");

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null!);
        _roleManagerMock.Setup(r => r.FindByNameAsync(AppRoles.EmployerRole)).ReturnsAsync((IdentityRole<Guid>)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User is not successfully registered. User Role is not successfully find");
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Error, "Employer role not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenCreateUserFails()
    {
        // Arrange
        var command = new RegisterEmployerCommand("employer", "Tech Corp", "employer@example.com", "P@ssw0rd123");
        var user = new AppUser { Id = Guid.NewGuid(), Email = command.Email };
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = AppRoles.EmployerRole };
        var errors = new[] { new IdentityError { Description = "Invalid password" } };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((AppUser)null!);
        _roleManagerMock.Setup(r => r.FindByNameAsync(AppRoles.EmployerRole)).ReturnsAsync(role);
        _mapperMock.Setup(m => m.Map<AppUser>(command)).Returns(user);
        _userManagerMock.Setup(m => m.CreateAsync(user, command.Password)).ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("User is not successfully registered. Errors: Invalid password");
        _employerProfilesRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployerProfile>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Error, "Failed to create employer: Invalid password", Times.Once());
    }
}