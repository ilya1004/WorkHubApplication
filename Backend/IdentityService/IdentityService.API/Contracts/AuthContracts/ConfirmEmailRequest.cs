namespace IdentityService.API.Contracts.AuthContracts;

public sealed record ConfirmEmailRequest(string Email, string Code);