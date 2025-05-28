namespace ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;

public class GetMyFreelancerApplicationsByFilterSpecification : Specification<FreelancerApplication>
{
    public GetMyFreelancerApplicationsByFilterSpecification(
        Guid freelancerId, 
        DateTime? startDate, 
        DateTime? endDate, 
        ApplicationStatus? applicationStatus, 
        int offset, 
        int limit) 
        : base(fa => 
            fa.FreelancerId == freelancerId && 
            (!startDate.HasValue || startDate.Value <= fa.CreatedAt) && 
            (!endDate.HasValue || fa.CreatedAt <= endDate.Value.AddDays(1)) && 
            (!applicationStatus.HasValue || fa.Status == applicationStatus))
    {
        AddInclude(fa => fa.Project);
        AddOrderByDescending(fa => fa.CreatedAt);
        AddPagination(offset, limit);
    }
}