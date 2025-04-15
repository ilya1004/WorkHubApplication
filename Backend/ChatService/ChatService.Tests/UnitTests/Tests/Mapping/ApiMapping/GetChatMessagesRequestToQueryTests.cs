using ChatService.API.Contracts.ChatContracts;
using ChatService.API.Mapping.MessageMappingProfiles;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;

namespace ChatService.Tests.UnitTests.Tests.Mapping.ApiMapping;

public class GetChatMessagesRequestToQueryTests
{
    private readonly IMapper _mapper;

    public GetChatMessagesRequestToQueryTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GetChatMessagesRequestToQuery>());
        _mapper = config.CreateMapper();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_GetChatMessagesRequestToQuery_ShouldMapAllProperties()
    {
        // Arrange
        var request = new GetChatMessagesRequest(
            ChatId: Guid.NewGuid(),
            PageNo: 2,
            PageSize: 25);

        // Act
        var query = _mapper.Map<GetChatMessagesQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.ChatId.Should().Be(request.ChatId);
        query.PageNo.Should().Be(request.PageNo);
        query.PageSize.Should().Be(request.PageSize);
    }
}