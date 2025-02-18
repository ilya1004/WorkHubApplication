namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;

public class GetFreelancerApplicationByIdQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetFreelancerApplicationByIdQuery, FreelancerApplication>
{
    public async Task<FreelancerApplication> Handle(GetFreelancerApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken);
        
        if (freelancerApplication is null)
        {
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }

        if (freelancerApplication.FreelancerId != request.FreelancerId)
        {
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }
        
        return freelancerApplication;
    }
}