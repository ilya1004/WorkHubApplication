
namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;

public  class UpdateEmployerIndustryCommandHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper) : IRequestHandler<UpdateEmployerIndustryCommand>
{
    public async Task Handle(UpdateEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id);

        if (industry == null)
        {
            throw new NotFoundException($"Employer industry with ID '{request.Id}' not found");
        }

        mapper.Map(request, industry);

        await unitOfWork.EmployerIndustriesRepository.UpdateAsync(industry, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
