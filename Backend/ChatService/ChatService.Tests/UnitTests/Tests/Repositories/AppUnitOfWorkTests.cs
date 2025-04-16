
using ChatService.Infrastructure.Repositories;
using ChatService.Infrastructure.Settings;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.Options;

namespace ChatService.Tests.UnitTests.Tests.Repositories;

public class AppUnitOfWorkTests : MongoTestBase
{
    private readonly AppUnitOfWork _unitOfWork;

    public AppUnitOfWorkTests()
    {
        var optionsMock = new Mock<IOptions<MongoDbSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new MongoDbSettings
        {
            DatabaseName = "TestDatabase",
            ConnectionString = null!
        });
        _unitOfWork = new AppUnitOfWork(Client, optionsMock.Object);
    }

    [Fact]
    public void MessagesRepository_ShouldReturnMessagesRepository()
    {
        var repository = _unitOfWork.MessagesRepository;
        repository.Should().NotBeNull();
        repository.Should().BeOfType<MessagesRepository>();
    }

    [Fact]
    public void ChatRepository_ShouldReturnChatsRepository()
    {
        var repository = _unitOfWork.ChatRepository;
        repository.Should().NotBeNull();
        repository.Should().BeOfType<ChatsRepository>();
    }

    [Fact]
    public void MessagesRepository_ShouldBeSingletonPerUnitOfWork()
    {
        var repo1 = _unitOfWork.MessagesRepository;
        var repo2 = _unitOfWork.MessagesRepository;
        repo1.Should().BeSameAs(repo2);
    }

    [Fact]
    public void ChatRepository_ShouldBeSingletonPerUnitOfWork()
    {
        var repo1 = _unitOfWork.ChatRepository;
        var repo2 = _unitOfWork.ChatRepository;
        repo1.Should().BeSameAs(repo2);
    }
}