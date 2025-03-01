using MediatR;
using PaymentsService.Domain.DTOs;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries;

public sealed record GetEmployerAccountQuery : IRequest<EmployerAccountDto>;