namespace ProjectsService.API.Contracts.ProjectContracts;

public sealed record CreateProjectRequest(ProjectDto Project, LifecycleDto Lifecycle);