using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Primitives;
using System.Threading;

namespace IdentityService.DAL.Repository;

public class EmployersRepository(ApplicationDbContext context) : AppRepository<EmployerProfile>(context), IEmployersRepository
{

}
