using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

public sealed record GetFreelancerAccountQuery : IRequest<FreelancerAccountModel>;