using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;

public class DeleteFreelancerSkillCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteFreelancerSkillCommand>
{
    public async Task Handle(DeleteFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        var freelancerSkill = await unitOfWork.FreelancerSkillsRepository.GetByIdAsync(request.Id);

        if (freelancerSkill is null) throw new NotFoundException($"Freelancer skill with ID '{request.Id}' not found");

        await unitOfWork.FreelancerSkillsRepository.DeleteAsync(freelancerSkill, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}