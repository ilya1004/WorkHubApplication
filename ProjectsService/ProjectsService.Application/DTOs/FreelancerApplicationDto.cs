namespace ProjectsService.Application.DTOs;

public record FreelancerApplicationDto(
    Guid ProjectId,
    Guid FreelancerId);