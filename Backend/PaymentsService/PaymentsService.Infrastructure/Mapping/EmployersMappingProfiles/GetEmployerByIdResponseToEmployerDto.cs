using PaymentsService.Infrastructure.DTOs;

namespace PaymentsService.Infrastructure.Mapping.EmployersMappingProfiles;

public class GetEmployerByIdResponseToEmployerDto : Profile
{
    public GetEmployerByIdResponseToEmployerDto()
    {
        CreateMap<Employers.GetEmployerByIdResponse, EmployerDto>()
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EmployerCustomerId, opt => 
                opt.MapFrom(src => src.EmployerCustomerId));
    }
}