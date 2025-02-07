using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repository;

public class AppUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Lazy<IEmployersRepository> _employersRepository;
    private readonly Lazy<IFreelancersRepository> _freelancersRepository;
    public AppUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _employersRepository = new Lazy<IEmployersRepository>(() => new EmployersRepository(context));
        _freelancersRepository = new Lazy<IFreelancersRepository>(() => new FreelancersRepository(context));
    }
    
    public IEmployersRepository EmployersRepository => _employersRepository.Value;
    public IFreelancersRepository FreelancersRepository => _freelancersRepository.Value;

    public async Task SaveAllAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
