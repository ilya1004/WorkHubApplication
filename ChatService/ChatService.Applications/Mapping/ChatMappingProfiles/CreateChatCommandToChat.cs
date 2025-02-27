using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateChat;

namespace ChatService.Applications.Mapping.ChatMappingProfiles;

public class CreateChatCommandToChat : Profile
{
    public CreateChatCommandToChat()
    {
        CreateMap<CreateChatCommand, Chat>()
            .ForMember(dest => dest.Id, opt => 
                opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => 
                opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}