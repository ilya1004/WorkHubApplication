using System.Text;
using System.Text.Json;
using IdentityService.DAL.Services.RedisService;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityService.Tests.UnitTests.Tests.Services.DalServices;

public class RedisServiceTests
{
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly RedisService _service;

    public RedisServiceTests()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _service = new RedisService(_distributedCacheMock.Object);
    }

    [Fact]
    public async Task SetAsync_ShouldCallSetAsync_WithCorrectParameters()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var expiry = TimeSpan.FromMinutes(10);
        DistributedCacheEntryOptions capturedOptions = null!;

        _distributedCacheMock.Setup(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == value),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, _, options, _) => capturedOptions = options)
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _service.SetAsync(key, value, expiry, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _distributedCacheMock.Verify(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == value),
            It.IsAny<DistributedCacheEntryOptions>(),
            CancellationToken.None), Times.Once());
        capturedOptions.Should().NotBeNull();
        capturedOptions.AbsoluteExpirationRelativeToNow.Should().Be(expiry);
    }

    [Fact]
    public async Task SetAsync_ShouldCallSetAsync_WithoutExpiry_WhenExpiryIsNull()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        DistributedCacheEntryOptions capturedOptions = null!;

        _distributedCacheMock.Setup(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == value),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, _, options, _) => capturedOptions = options)
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _service.SetAsync(key, value, null, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _distributedCacheMock.Verify(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == value),
            It.IsAny<DistributedCacheEntryOptions>(),
            CancellationToken.None), Times.Once());
        capturedOptions.Should().NotBeNull();
        capturedOptions.AbsoluteExpirationRelativeToNow.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnValue_WhenKeyExists()
    {
        // Arrange
        var key = "test-key";
        var expectedValue = "test-value";
        var valueBytes = Encoding.UTF8.GetBytes(expectedValue);

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueBytes);

        // Act
        var act = async () => await _service.GetAsync(key, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(expectedValue);
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "test-key";

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        // Act
        var act = async () => await _service.GetAsync(key, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeNull();
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var key = "test-key";
        var value = "some-value";
        var valueBytes = Encoding.UTF8.GetBytes(value);

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueBytes);

        // Act
        var act = async () => await _service.ExistsAsync(key, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeTrue();
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "test-key";

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        // Act
        var act = async () => await _service.ExistsAsync(key, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeFalse();
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRemoveAsync_WithCorrectKey()
    {
        // Arrange
        var key = "test-key";

        _distributedCacheMock.Setup(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _service.DeleteAsync(key, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _distributedCacheMock.Verify(c => c.RemoveAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task SetObjectAsync_ShouldSerializeAndCallSetAsync_WithCorrectParameters()
    {
        // Arrange
        var key = "test-key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var expiry = TimeSpan.FromHours(1);
        var json = JsonSerializer.Serialize(value);
        DistributedCacheEntryOptions capturedOptions = null!;

        _distributedCacheMock.Setup(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == json),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, _, options, _) => capturedOptions = options)
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _service.SetObjectAsync(key, value, expiry, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _distributedCacheMock.Verify(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == json),
            It.IsAny<DistributedCacheEntryOptions>(),
            CancellationToken.None), Times.Once());
        capturedOptions.Should().NotBeNull();
        capturedOptions.AbsoluteExpirationRelativeToNow.Should().Be(expiry);
    }

    [Fact]
    public async Task GetObjectAsync_ShouldReturnDeserializedObject_WhenKeyExists()
    {
        // Arrange
        var key = "test-key";
        var expectedObject = new TestObject { Id = 1, Name = "Test" };
        var json = JsonSerializer.Serialize(expectedObject);
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonBytes);

        // Act
        var act = async () => await _service.GetObjectAsync<TestObject>(key, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeEquivalentTo(expectedObject);
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task GetObjectAsync_ShouldReturnDefault_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "test-key";

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null!);

        // Act
        var act = async () => await _service.GetObjectAsync<TestObject>(key, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().BeNull();
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task GetObjectAsync_ShouldThrowJsonException_WhenInvalidJson()
    {
        // Arrange
        var key = "test-key";
        var invalidJson = "invalid-json";
        var invalidJsonBytes = Encoding.UTF8.GetBytes(invalidJson);

        _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidJsonBytes);

        // Act
        var act = async () => await _service.GetObjectAsync<TestObject>(key, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
        _distributedCacheMock.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once());
    }
}

public class TestObject
{
    public int Id { get; set; }
    public string Name { get; set; }
}