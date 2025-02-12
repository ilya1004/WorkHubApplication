using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Repository;

public class FreelancersRepository(ApplicationDbContext context) : AppRepository<FreelancerProfile>(context), IFreelancersRepository
{

}
