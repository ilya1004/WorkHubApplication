using ProjectsService.API.Contracts.FreelancerApplicationContracts;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreelancerApplicationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateFreelancerApplication([FromBody] CreateFreelancerApplicationRequest request, CancellationToken cancellationToken = default)
    {
        
    }
}