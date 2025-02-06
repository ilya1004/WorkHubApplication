using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repository;

public class AppUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Lazy<IEmployersRepository> _employersRepository;
    public AppUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _employersRepository = new Lazy<IEmployersRepository>(() => new EmployersRepository(context));
    }

    public IEmployersRepository EmployersRepository => _employersRepository.Value;

    public async Task SaveAllAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
