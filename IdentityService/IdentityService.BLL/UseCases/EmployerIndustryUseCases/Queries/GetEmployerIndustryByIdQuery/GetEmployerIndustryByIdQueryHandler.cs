
namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryByIdQuery;

public class GetEmployerIndustryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetEmployerIndustryByIdQuery, EmployerIndustry>
{
    public async Task<EmployerIndustry> Handle(GetEmployerIndustryByIdQuery request, CancellationToken cancellationToken)
    {
        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id);

        if (industry == null)
        {
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        return industry;
    }
}
