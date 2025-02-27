using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.ChatUseCases.Queries.GetChatMessages;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class GetChatMessagesRequestToQuery : Profile
{
    public GetChatMessagesRequestToQuery()
    {
        CreateMap<GetChatMessagesRequest, GetChatMessagesQuery>();
    }
}