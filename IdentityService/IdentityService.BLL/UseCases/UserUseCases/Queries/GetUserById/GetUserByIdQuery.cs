namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<AppUser>;