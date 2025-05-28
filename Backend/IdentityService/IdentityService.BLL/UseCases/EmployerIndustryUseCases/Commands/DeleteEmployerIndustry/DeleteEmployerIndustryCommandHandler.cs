namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;

public class DeleteEmployerIndustryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteEmployerIndustryCommandHandler> logger) : IRequestHandler<DeleteEmployerIndustryCommand>
{
    public async Task Handle(DeleteEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting employer industry with ID: {IndustryId}", request.Id);

        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            logger.LogWarning("Employer industry with ID {IndustryId} not found", request.Id);
            
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        await unitOfWork.EmployerIndustriesRepository.DeleteAsync(industry, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully deleted industry with ID: {IndustryId}", request.Id);
    }
}