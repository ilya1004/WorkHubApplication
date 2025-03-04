namespace ProjectsService.Application.Specifications.ProjectSpecifications;

public class GetProjectsByEmployerFilterSpecification : Specification<Project>
{
    public GetProjectsByEmployerFilterSpecification(
        Guid employerId,
        DateTime? updatedAtStartDate,
        DateTime? updatedAtEndDate,
        ProjectStatus? projectStatus,
        bool? acceptanceRequestedAndNotConfirmed,
        int offset,
        int limit)
        : base(p =>
            p.EmployerId == employerId &&
            (!updatedAtStartDate.HasValue || updatedAtStartDate.Value <= p.Lifecycle.UpdatedAt) && 
            (!updatedAtEndDate.HasValue || p.Lifecycle.UpdatedAt <= updatedAtEndDate.Value.AddDays(1)) &&
            (!projectStatus.HasValue || p.Lifecycle.Status == projectStatus.Value) &&
            (!acceptanceRequestedAndNotConfirmed.HasValue || 
             p.Lifecycle.AcceptanceRequested == acceptanceRequestedAndNotConfirmed 
             && p.Lifecycle.AcceptanceConfirmed == !acceptanceRequestedAndNotConfirmed))
    {
        AddInclude(p => p.Category!);
        AddInclude(p => p.Lifecycle);
        AddOrderByDescending(p => p.Lifecycle.CreatedAt);
        AddPagination(offset, limit);
    }
}