namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;

public class GetEmployerIndustryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetEmployerIndustryByIdQueryHandler> logger) : IRequestHandler<GetEmployerIndustryByIdQuery, EmployerIndustry>
{
    public async Task<EmployerIndustry> Handle(GetEmployerIndustryByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting employer industry by ID: {IndustryId}", request.Id);

        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            logger.LogWarning("Employer industry with ID {IndustryId} not found", request.Id);
            
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        logger.LogInformation("Successfully retrieved industry with ID: {IndustryId}", request.Id);
        
        return industry;
    }
}