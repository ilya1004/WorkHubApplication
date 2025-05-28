namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;

public class UpdateEmployerIndustryCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<UpdateEmployerIndustryCommandHandler> logger) : IRequestHandler<UpdateEmployerIndustryCommand>
{
    public async Task Handle(UpdateEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating employer industry with ID: {IndustryId}", request.Id);

        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            logger.LogWarning("Employer industry with ID {IndustryId} not found", request.Id);
            
            throw new NotFoundException($"Employer industry with ID '{request.Id}' not found");
        }

        logger.LogInformation("Mapping changes to industry with ID: {IndustryId}", request.Id);
        
        mapper.Map(request, industry);

        await unitOfWork.EmployerIndustriesRepository.UpdateAsync(industry, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully updated industry with ID: {IndustryId}", request.Id);
    }
}