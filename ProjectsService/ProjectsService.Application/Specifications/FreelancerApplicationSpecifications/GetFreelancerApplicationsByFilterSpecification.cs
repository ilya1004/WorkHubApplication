using System.Linq.Expressions;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;

public class GetFreelancerApplicationsByFilterSpecification : Specification<FreelancerApplication>
{
    public GetFreelancerApplicationsByFilterSpecification(
        Guid freelancerId, 
        DateTime? startDate, 
        DateTime? endDate, 
        ApplicationStatus? applicationStatus, 
        int offset, 
        int limit) 
        : base(fa => 
            fa.FreelancerId == freelancerId &&
            (!startDate.HasValue || !endDate.HasValue || 
             startDate.Value <= fa.CreatedAt && fa.CreatedAt <= endDate.Value.AddDays(1)) &&
            (!applicationStatus.HasValue || fa.Status == applicationStatus))
    {
        AddOrderByDescending(fa => fa.CreatedAt);
        AddPagination(offset, limit);
        AddInclude(fa => fa.Project);
    }
}