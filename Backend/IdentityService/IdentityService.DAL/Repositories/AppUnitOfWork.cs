using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace IdentityService.DAL.Repositories;

public class AppUnitOfWork(
    ApplicationDbContext context,
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : IUnitOfWork
{
    private readonly Lazy<IRepository<EmployerProfile>> _employerProfilesRepository =
        new(() => new CachedAppRepository<EmployerProfile>(new AppRepository<EmployerProfile>(context), distributedCache, options));

    private readonly Lazy<IRepository<FreelancerProfile>> _freelancerProfilesRepository =
        new(() => new CachedAppRepository<FreelancerProfile>(new AppRepository<FreelancerProfile>(context), distributedCache, options));

    private readonly Lazy<IRepository<FreelancerSkill>> _freelancerSkillsRepository =
        new(() => new CachedAppRepository<FreelancerSkill>(new AppRepository<FreelancerSkill>(context), distributedCache, options));

    private readonly Lazy<IRepository<EmployerIndustry>> _employerIndustriesRepository =
        new(() => new CachedAppRepository<EmployerIndustry>(new AppRepository<EmployerIndustry>(context), distributedCache, options));

    private readonly Lazy<IUsersRepository> _usersRepository =
        new(() => new CachedUsersRepository(new UsersRepository(context), distributedCache, options));

    public IRepository<EmployerProfile> EmployerProfilesRepository => _employerProfilesRepository.Value;
    public IRepository<FreelancerProfile> FreelancerProfilesRepository => _freelancerProfilesRepository.Value;
    public IRepository<FreelancerSkill> FreelancerSkillsRepository => _freelancerSkillsRepository.Value;
    public IRepository<EmployerIndustry> EmployerIndustriesRepository => _employerIndustriesRepository.Value;
    public IUsersRepository UsersRepository => _usersRepository.Value;

    public async Task SaveAllAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}