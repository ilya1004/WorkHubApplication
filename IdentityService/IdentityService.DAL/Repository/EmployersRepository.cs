using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Primitives;
using System.Threading;

namespace IdentityService.DAL.Repository;

public class EmployersRepository : AppRepository<EmployerProfile>, IEmployersRepository
{
    public EmployersRepository(ApplicationDbContext context) : base(context) { }

    //public async Task CreateProfile(EmployerProfile employerProfile, CancellationToken cancellationToken)
    //{
    //    await _entities.AddAsync(employerProfile, cancellationToken);
    //}
}
