using Employers;

namespace PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;

public class GetEmployerByIdResponseToEmployerDto : Profile
{
    public GetEmployerByIdResponseToEmployerDto()
    {
        CreateMap<GetEmployerByIdResponse, EmployerDto>()
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EmployerCustomerId, opt => 
                opt.MapFrom(src => src.EmployerCustomerId));
    }
}