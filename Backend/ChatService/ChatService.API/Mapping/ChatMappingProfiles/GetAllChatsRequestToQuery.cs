using ChatService.API.Contracts.ChatContracts;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;

namespace ChatService.API.Mapping.ChatMappingProfiles;

public class GetAllChatsRequestToQuery : Profile
{
    public GetAllChatsRequestToQuery()
    {
        CreateMap<GetAllChatsRequest, GetAllChatsQuery>();
    }
}