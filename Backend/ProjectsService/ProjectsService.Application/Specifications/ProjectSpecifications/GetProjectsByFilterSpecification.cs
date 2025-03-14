namespace ProjectsService.Application.Specifications.ProjectSpecifications;

public class GetProjectsByFilterSpecification : Specification<Project>
{
    public GetProjectsByFilterSpecification(
        string? title, 
        decimal? budgetFrom, 
        decimal? budgetTo,
        Guid? categoryId, 
        Guid? employerId,
        ProjectStatus? projectStatus,
        int offset, 
        int limit) 
        : base(p =>
            (string.IsNullOrEmpty(title) || p.Title.ToLower().Contains(title.ToLower())) &&
            (!budgetFrom.HasValue || p.Budget >= budgetFrom.Value) &&
            (!budgetTo.HasValue || p.Budget <= budgetTo.Value) &&
            (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
            (!employerId.HasValue || p.EmployerId == employerId.Value) &&
            (!projectStatus.HasValue || p.Lifecycle.Status == projectStatus.Value))
    {
        AddInclude(p => p.Lifecycle);
        AddInclude(p => p.Category!);
        AddOrderByDescending(p => p.Lifecycle.CreatedAt);
        AddPagination(offset, limit);
    }
}
