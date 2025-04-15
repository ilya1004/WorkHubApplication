using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FileUseCases;

public class GetImageByUserIdQueryHandlerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILogger<GetImageByUserIdQueryHandler>> _loggerMock;
    private readonly GetImageByUserIdQueryHandler _handler;

    public GetImageByUserIdQueryHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILogger<GetImageByUserIdQueryHandler>>();

        _handler = new GetImageByUserIdQueryHandler(_userManagerMock.Object, _blobServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFileResponse_WhenUserAndImageExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var command = new GetImageByUserIdQuery(userId);
        var user = new AppUser { Id = userId, ImageUrl = imageId.ToString() };
        var stream = new MemoryStream();
        var contentType = "image/jpeg";
        var fileResponse = new FileResponseDto(stream, contentType);

        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _blobServiceMock.Setup(b => b.DownloadAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileResponse);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(fileResponse);
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved image for user ID: {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new GetImageByUserIdQuery(userId);

        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((AppUser)null!);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID '{userId}' not found");
        _blobServiceMock.Verify(b => b.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID {userId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserHasNoImage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new GetImageByUserIdQuery(userId);
        var user = new AppUser { Id = userId, ImageUrl = null };

        _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with ID {userId} don't have an image");
        _blobServiceMock.Verify(b => b.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"User with ID {userId} doesn't have an image", Times.Once());
    }
}