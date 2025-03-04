using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;

public record GetAllFreelancerApplicationsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<FreelancerApplication>>;