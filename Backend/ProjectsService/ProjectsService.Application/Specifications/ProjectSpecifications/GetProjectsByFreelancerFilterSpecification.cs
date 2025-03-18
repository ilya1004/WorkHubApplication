namespace ProjectsService.Application.Specifications.ProjectSpecifications;

public class GetProjectsByFreelancerFilterSpecification : Specification<Project>
{
    public GetProjectsByFreelancerFilterSpecification(
        Guid freelancerId,
        ProjectStatus? projectStatus,
        Guid? employerId,
        int offset,
        int limit) 
        : base(p => 
            p.FreelancerId == freelancerId &&
            (!projectStatus.HasValue || p.Lifecycle.Status == projectStatus.Value) &&
            (!employerId.HasValue || p.EmployerId == employerId))
    {
        AddOrderByDescending(p => p.Lifecycle.CreatedAt);
        AddPagination(offset, limit);
    }
}