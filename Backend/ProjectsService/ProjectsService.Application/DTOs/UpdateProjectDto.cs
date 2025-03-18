namespace ProjectsService.Application.DTOs;

public record UpdateProjectDto(
    string Title,
    string? Description,
    decimal Budget,
    Guid? CategoryId);