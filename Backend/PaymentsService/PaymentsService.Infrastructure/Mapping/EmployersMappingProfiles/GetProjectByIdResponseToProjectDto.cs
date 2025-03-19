using Projects;

namespace PaymentsService.Infrastructure.Mapping.EmployersMappingProfiles;

public class GetProjectByIdResponseToProjectDto : Profile
{
    public GetProjectByIdResponseToProjectDto()
    {
        CreateMap<GetProjectByIdResponse, ProjectDto>();
    }
}