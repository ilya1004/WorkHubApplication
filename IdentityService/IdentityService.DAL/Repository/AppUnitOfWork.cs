using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Repository;

public class AppUnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly Lazy<IEmployersRepository> _employersRepository = 
        new(() => new EmployersRepository(context));

    private readonly Lazy<IFreelancersRepository> _freelancersRepository = 
        new(() => new FreelancersRepository(context));

    private readonly Lazy<IRepository<FreelancerSkill>> _freelancerSkillsRepository = 
        new(() => new AppRepository<FreelancerSkill>(context));

    private readonly Lazy<IRepository<EmployerIndustry>> _employerIndustriesRepository = 
        new(() => new AppRepository<EmployerIndustry>(context));

    private readonly Lazy<IUsersRepository> _usersRepository = 
        new(() => new UsersRepository(context));

    public IEmployersRepository EmployersRepository => _employersRepository.Value;
    public IFreelancersRepository FreelancersRepository => _freelancersRepository.Value;
    public IRepository<FreelancerSkill> FreelancerSkillsRepository => _freelancerSkillsRepository.Value;
    public IRepository<EmployerIndustry> EmployerIndustriesRepository => _employerIndustriesRepository.Value;
    public IUsersRepository UsersRepository => _usersRepository.Value;

    public async Task SaveAllAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
