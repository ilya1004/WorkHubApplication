namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;

public class DeleteFreelancerApplicationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteFreelancerApplicationCommand>
{
    public async Task Handle(DeleteFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.Id,
            cancellationToken);
        
        if (freelancerApplication is null)
        {
            throw new NotFoundException($"Freelancer Application with ID '{request.Id}' not found");
        }

        if (freelancerApplication.FreelancerId != request.FreelancerId)
        {
            throw new ForbiddenException($"You cannot delete this Freelancer Application with ID '{request.Id}'");
        }
        
        await unitOfWork.FreelancerApplicationCommandsRepository.DeleteAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}