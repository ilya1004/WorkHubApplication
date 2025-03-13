namespace IdentityService.API.Contracts.AuthContracts;

public sealed record RefreshTokenRequest(string AccessToken, string RefreshToken);