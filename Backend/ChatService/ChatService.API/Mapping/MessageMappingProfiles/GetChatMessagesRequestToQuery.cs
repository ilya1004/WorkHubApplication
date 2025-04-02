using ChatService.API.Contracts.ChatContracts;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class GetChatMessagesRequestToQuery : Profile
{
    public GetChatMessagesRequestToQuery()
    {
        CreateMap<GetChatMessagesRequest, GetChatMessagesQuery>();
    }
}