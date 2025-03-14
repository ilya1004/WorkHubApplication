using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

public sealed record GetFreelancerAccountQuery : IRequest<FreelancerAccountModel>;