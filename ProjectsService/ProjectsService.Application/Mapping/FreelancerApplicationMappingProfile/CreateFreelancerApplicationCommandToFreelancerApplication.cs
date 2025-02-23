using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

namespace ProjectsService.Application.Mapping.FreelancerApplicationMappingProfile;

public class CreateFreelancerApplicationCommandToFreelancerApplication : Profile
{
    public CreateFreelancerApplicationCommandToFreelancerApplication()
    {
        CreateMap<CreateFreelancerApplicationCommand, FreelancerApplication>()
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt =>
                opt.MapFrom(_ => ApplicationStatus.Pending));
    }
}