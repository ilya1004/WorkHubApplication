using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

public class CreateFreelancerApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserContext userContext) : IRequestHandler<CreateFreelancerApplicationCommand>
{
    public async Task Handle(CreateFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.FirstOrDefaultAsync(
            fa => fa.ProjectId == request.ProjectId && fa.FreelancerId == userId,
            cancellationToken);

        if (freelancerApplication is not null)
        {
            throw new AlreadyExistsException($"Freelancer application to the project with ID '{request.ProjectId}' already exists.");
        }
        
        var newFreelancerApplication = mapper.Map<FreelancerApplication>(request);
        
        await unitOfWork.FreelancerApplicationCommandsRepository.AddAsync(newFreelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}