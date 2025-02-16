using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;

public class DeleteFreelancerApplicationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteFreelancerApplicationCommand>
{
    public async Task Handle(DeleteFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.FirstOrDefaultAsync(
            fa => fa.FreelancerId == request.FreelancerId && fa.ProjectId == request.ProjectId,
            cancellationToken);

        if (freelancerApplication is null)
        {
            throw new NotFoundException($"Freelancer Application with freelancer ID '{request.FreelancerId}' " +
                                        $"and project ID '{request.ProjectId}' not found");
        }

        if (freelancerApplication.Status != ApplicationStatus.Pending)
        {
            throw new BadRequestException("You can not remove Freelancer Application when its not pending.");
        }
        
        await unitOfWork.FreelancerApplicationCommandsRepository.DeleteAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}