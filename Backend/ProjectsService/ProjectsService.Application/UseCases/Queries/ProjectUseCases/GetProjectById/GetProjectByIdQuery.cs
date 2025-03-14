namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Project>;