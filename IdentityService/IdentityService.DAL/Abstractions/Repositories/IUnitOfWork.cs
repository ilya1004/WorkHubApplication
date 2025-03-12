using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUnitOfWork
{
    public IEmployersRepository EmployersRepository { get; }
    public IFreelancersRepository FreelancersRepository { get; }
    public IRepository<FreelancerSkill> FreelancerSkillsRepository { get; }
    public IRepository<EmployerIndustry> EmployerIndustriesRepository { get; }
    public IUsersRepository UsersRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}