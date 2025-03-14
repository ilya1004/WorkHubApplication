using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

public class CreateEmployerIndustryCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateEmployerIndustryCommand>
{
    public async Task Handle(CreateEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        var industry = await unitOfWork.EmployerIndustriesRepository.FirstOrDefaultAsync(
            ei => ei.Name == request.Name,
            cancellationToken);

        if (industry != null) throw new BadRequestException($"Industry with the name '{request.Name}' already exists.");

        var newIndustry = mapper.Map<EmployerIndustry>(request);

        await unitOfWork.EmployerIndustriesRepository.AddAsync(newIndustry, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}