namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;

public class DeleteFreelancerApplicationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteFreelancerApplicationCommand>
{
    public async Task Handle(DeleteFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);
        
        if (freelancerApplication is null)
        {
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }

        if (freelancerApplication.FreelancerId != request.FreelancerId)
        {
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }
        
        await unitOfWork.FreelancerApplicationCommandsRepository.DeleteAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}