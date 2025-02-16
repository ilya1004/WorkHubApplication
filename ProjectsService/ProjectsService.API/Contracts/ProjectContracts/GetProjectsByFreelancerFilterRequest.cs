using ProjectsService.Domain.Enums;

namespace ProjectsService.API.Contracts.ProjectContracts;

public sealed record GetProjectsByFreelancerFilterRequest(
    Guid FreelancerId,
    ProjectStatus? ProjectStatus,
    Guid? EmployerId,
    int PageNo = 1,
    int PageSize = 10);