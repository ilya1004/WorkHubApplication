
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;

public class DeleteEmployerIndustryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteEmployerIndustryCommand>
{
    public async Task Handle(DeleteEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        await unitOfWork.EmployerIndustriesRepository.DeleteAsync(industry, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
