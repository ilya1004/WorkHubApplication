namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

public class CreateEmployerIndustryCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<CreateEmployerIndustryCommandHandler> logger) : IRequestHandler<CreateEmployerIndustryCommand>
{
    public async Task Handle(CreateEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new employer industry with name: {IndustryName}", request.Name);

        var industry = await unitOfWork.EmployerIndustriesRepository.FirstOrDefaultAsync(
            ei => ei.Name == request.Name,
            cancellationToken);

        if (industry != null)
        {
            logger.LogWarning("Industry with name {IndustryName} already exists", request.Name);
            
            throw new BadRequestException($"Industry with the name '{request.Name}' already exists.");
        }

        logger.LogInformation("Mapping and creating new industry");
        
        var newIndustry = mapper.Map<EmployerIndustry>(request);

            await unitOfWork.EmployerIndustriesRepository.AddAsync(newIndustry, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully created new industry with ID: {IndustryId}", newIndustry.Id);
    }
}