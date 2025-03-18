namespace ProjectsService.Application.DTOs;

public record LifecycleDto(
    DateTime ApplicationsStartDate,
    DateTime ApplicationsDeadline,
    DateTime WorkStartDate,
    DateTime WorkDeadline);