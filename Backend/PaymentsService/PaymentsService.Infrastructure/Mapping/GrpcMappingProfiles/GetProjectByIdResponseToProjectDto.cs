using Projects;

namespace PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;

public class GetProjectByIdResponseToProjectDto : Profile
{
    public GetProjectByIdResponseToProjectDto()
    {
        CreateMap<GetProjectByIdResponse, ProjectDto>();
    }
}