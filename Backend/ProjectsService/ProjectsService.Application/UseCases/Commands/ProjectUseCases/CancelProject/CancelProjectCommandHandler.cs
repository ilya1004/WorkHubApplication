using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;

public class CancelProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IPaymentsProducerService paymentsProducerService) : IRequestHandler<CancelProjectCommand>
{
    public async Task Handle(CancelProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken, 
            p => p.Lifecycle);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (project.EmployerId != userId)
        {
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        project.Lifecycle.Status = ProjectStatus.Cancelled;
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        if (project.PaymentIntentId is not null)
        {
            await paymentsProducerService.CancelPaymentAsync(project.PaymentIntentId, cancellationToken);    
        }
    }
}