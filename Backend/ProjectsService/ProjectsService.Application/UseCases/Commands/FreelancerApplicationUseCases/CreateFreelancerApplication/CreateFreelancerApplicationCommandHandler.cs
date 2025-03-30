using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

public class CreateFreelancerApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserContext userContext,
    ILogger<CreateFreelancerApplicationCommandHandler> logger) : IRequestHandler<CreateFreelancerApplicationCommand>
{
    public async Task Handle(CreateFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("User {UserId} creating application for project {ProjectId}", userId, request.ProjectId);
        
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.FirstOrDefaultAsync(
            fa => fa.ProjectId == request.ProjectId && fa.FreelancerId == userId,
            cancellationToken);

        if (freelancerApplication is not null)
        {
            logger.LogWarning("User {UserId} already has application for project {ProjectId}", userId, request.ProjectId);
            
            throw new AlreadyExistsException($"Freelancer application to the project with ID '{request.ProjectId}' already exists.");
        }
        
        var newFreelancerApplication = mapper.Map<FreelancerApplication>(request);
        newFreelancerApplication.FreelancerId = userId;
        
        logger.LogInformation("Adding new freelancer application for project {ProjectId}", request.ProjectId);
        
        await unitOfWork.FreelancerApplicationCommandsRepository.AddAsync(newFreelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully created freelancer application for project {ProjectId}", request.ProjectId);
    }
}