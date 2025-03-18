namespace PaymentsService.Infrastructure.DTOs;

public record EmployerDto
{
    public string Id { get; init; }
    public string? EmployerCustomerId { get; init; }
}