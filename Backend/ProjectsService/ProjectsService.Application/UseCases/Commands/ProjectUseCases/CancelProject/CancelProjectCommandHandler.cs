using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;

public class CancelProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IPaymentsProducerService paymentsProducerService,
    ILogger<CancelProjectCommandHandler> logger) : IRequestHandler<CancelProjectCommand>
{
    public async Task Handle(CancelProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting cancellation of project {ProjectId}", request.ProjectId);

        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken, 
            p => p.Lifecycle);

        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found", request.ProjectId);
            
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (project.EmployerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to cancel project {ProjectId} without permission", userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        logger.LogInformation("Cancelling project {ProjectId}", request.ProjectId);
        
        project.Lifecycle.Status = ProjectStatus.Cancelled;
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        if (project.PaymentIntentId is not null)
        {
            logger.LogInformation("Sending payment cancellation for project {ProjectId}, payment intent {PaymentIntentId}", 
                request.ProjectId, project.PaymentIntentId);
            
            await paymentsProducerService.CancelPaymentAsync(project.PaymentIntentId, cancellationToken);    
        }

        logger.LogInformation("Successfully cancelled project {ProjectId}", request.ProjectId);
    }
}