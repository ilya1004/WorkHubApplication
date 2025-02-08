using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Abstractions.Data;

public interface IUnitOfWork
{
    public IEmployersRepository EmployersRepository { get; }
    public IFreelancersRepository FreelancersRepository { get; }
    public IRepository<FreelancerSkill> FreelancerSkillsRepository { get; }
    public IRepository<EmployerIndustry> EmployerIndustriesRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}
