namespace ProjectsService.Application.DTOs;

public record ProjectDto(
    string Title,
    string? Description,
    decimal Budget,
    Guid? CategoryId);