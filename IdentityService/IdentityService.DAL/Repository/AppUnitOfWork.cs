using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repository;

public class AppUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public AppUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveAllAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
