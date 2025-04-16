using ChatService.Infrastructure.Services.DbInitializer;
using ChatService.Infrastructure.Settings;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;

namespace ChatService.Tests.UnitTests.Tests.Services.InfrastructureServices;

public class DbInitializerTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly Mock<ILogger<DbInitializer>> _loggerMock;
    private readonly Mock<IOptions<MongoDbSettings>> _optionsMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly DbInitializer _initializer;

    public DbInitializerTests()
    {
        _runner = MongoDbRunner.Start();
        _loggerMock = new Mock<ILogger<DbInitializer>>();
        _optionsMock = new Mock<IOptions<MongoDbSettings>>();
        _configurationMock = new Mock<IConfiguration>();

        _optionsMock.Setup(o => o.Value).Returns(new MongoDbSettings
        {
            ConnectionString = _runner.ConnectionString,
            DatabaseName = "TestDatabase"
        });

        _initializer = new DbInitializer(_optionsMock.Object, _loggerMock.Object);
    }

    public void Dispose()
    {
        _runner.Dispose();
    }

    [Fact]
    public async Task InitializeDbAsync_ShouldCreateCollections_WhenTheyDoNotExist()
    {
        // Arrange
        var database = new MongoClient(_runner.ConnectionString).GetDatabase("TestDatabase");

        // Act
        await _initializer.InitializeDbAsync(_configurationMock.Object);

        // Assert
        var collections = (await database.ListCollectionNamesAsync()).ToList();
        collections.Should().Contain("Chats");
        collections.Should().Contain("Messages");

        _loggerMock.VerifyLog(LogLevel.Information, "Starting database initialization", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Connected to MongoDB database: TestDatabase", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating 'Chats' collection", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating 'Messages' collection", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Database initialization completed successfully", Times.Once());
    }

    [Fact]
    public async Task InitializeDbAsync_ShouldNotCreateCollections_WhenTheyExist()
    {
        // Arrange
        var database = new MongoClient(_runner.ConnectionString).GetDatabase("TestDatabase");
        await database.CreateCollectionAsync("Chats");
        await database.CreateCollectionAsync("Messages");

        // Act
        await _initializer.InitializeDbAsync(_configurationMock.Object);

        // Assert
        var collections = (await database.ListCollectionNamesAsync()).ToList();
        collections.Should().Contain("Chats");
        collections.Should().Contain("Messages");

        _loggerMock.VerifyLog(LogLevel.Information, "Starting database initialization", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Connected to MongoDB database: TestDatabase", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating 'Chats' collection", Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating 'Messages' collection", Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, "Database initialization completed successfully", Times.Once());
    }

    [Fact]
    public async Task InitializeDbAsync_ShouldThrowException_WhenMongoDbFails()
    {
        // Arrange
        _optionsMock.Setup(o => o.Value).Returns(new MongoDbSettings
        {
            ConnectionString = "mongodb://invalid:27017",
            DatabaseName = "TestDatabase"
        });

        // Act
        var act = async () => await _initializer.InitializeDbAsync(_configurationMock.Object);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Database initialization failed");

        _loggerMock.VerifyLog(LogLevel.Information, "Starting database initialization", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
    }
}