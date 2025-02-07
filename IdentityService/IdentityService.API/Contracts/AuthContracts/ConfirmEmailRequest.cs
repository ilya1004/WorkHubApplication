namespace IdentityService.API.Contracts.AuthContracts;

public sealed record ConfirmEmailRequest(int UserId, string Token);
