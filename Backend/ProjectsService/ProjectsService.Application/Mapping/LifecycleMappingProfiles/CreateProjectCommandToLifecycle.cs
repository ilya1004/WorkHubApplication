using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

namespace ProjectsService.Application.Mapping.LifecycleMappingProfiles;

public class CreateProjectCommandToLifecycle : Profile
{
    public CreateProjectCommandToLifecycle()
    {
        CreateMap<CreateProjectCommand, Lifecycle>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ApplicationsStartDate, opt =>
                opt.MapFrom(src => src.Lifecycle.ApplicationsStartDate))
            .ForMember(dest => dest.ApplicationsDeadline, opt =>
                opt.MapFrom(src => src.Lifecycle.ApplicationsDeadline))
            .ForMember(dest => dest.WorkStartDate, opt =>
                opt.MapFrom(src => src.Lifecycle.WorkStartDate))
            .ForMember(dest => dest.WorkDeadline, opt =>
                opt.MapFrom(src => src.Lifecycle.WorkDeadline))
            .ForMember(dest => dest.Project, opt =>
                opt.Ignore())
            .ForMember(dest => dest.Status, opt =>
                opt.MapFrom(_ => ProjectStatus.Published));
    }
}