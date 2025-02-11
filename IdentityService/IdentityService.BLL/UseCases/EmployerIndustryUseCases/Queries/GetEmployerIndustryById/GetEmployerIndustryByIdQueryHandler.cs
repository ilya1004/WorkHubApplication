using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;

public class GetEmployerIndustryByIdQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetEmployerIndustryByIdQuery, EmployerIndustry>
{
    public async Task<EmployerIndustry> Handle(GetEmployerIndustryByIdQuery request, CancellationToken cancellationToken)
    {
        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id);

        if (industry is null)
        {
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        return industry;
    }
}
