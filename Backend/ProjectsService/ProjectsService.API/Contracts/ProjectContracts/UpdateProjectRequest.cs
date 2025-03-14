namespace ProjectsService.API.Contracts.ProjectContracts;

public sealed record UpdateProjectRequest(UpdateProjectDto Project, LifecycleDto Lifecycle);