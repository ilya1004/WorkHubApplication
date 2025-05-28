using Projects;

namespace PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;

public class GetProjectByIdResponseToProjectDto : Profile
{
    public GetProjectByIdResponseToProjectDto()
    {
        CreateMap<GetProjectByIdResponse, ProjectDto>()
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.BudgetInCents, opt => 
                opt.MapFrom(src => src.BudgetInCents))
            .ForMember(dest => dest.FreelancerId, opt => 
                opt.MapFrom(src => string.IsNullOrEmpty(src.FreelancerId) ? (Guid?)null : Guid.Parse(src.FreelancerId)))
            .ForMember(dest => dest.PaymentIntentId, opt => 
                opt.MapFrom(src => src.PaymentIntentId));
    }
}