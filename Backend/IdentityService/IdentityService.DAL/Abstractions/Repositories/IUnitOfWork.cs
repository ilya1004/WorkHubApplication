using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUnitOfWork
{
    public IRepository<EmployerProfile> EmployerProfilesRepository { get; }
    public IRepository<FreelancerProfile> FreelancerProfilesRepository { get; }
    public IRepository<FreelancerSkill> FreelancerSkillsRepository { get; }
    public IRepository<EmployerIndustry> EmployerIndustriesRepository { get; }
    public IUsersRepository UsersRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}