using ProjectsService.Domain.Enums;

namespace ProjectsService.API.Contracts.FreelancerApplicationContracts;

public sealed record GetMyFreelancerApplicationsByFilterRequest(
    DateTime? StartDate,
    DateTime? EndDate,
    ApplicationStatus? ApplicationStatus,
    int PageNo = 1,
    int PageSize = 10);