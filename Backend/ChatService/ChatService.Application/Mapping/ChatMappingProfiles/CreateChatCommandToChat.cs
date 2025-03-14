using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;

namespace ChatService.Application.Mapping.ChatMappingProfiles;

public class CreateChatCommandToChat : Profile
{
    public CreateChatCommandToChat()
    {
        CreateMap<CreateChatCommand, Chat>()
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsActive, opt => 
                opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}