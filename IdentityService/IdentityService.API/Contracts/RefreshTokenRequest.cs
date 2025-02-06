namespace IdentityService.API.Contracts;

public sealed record RefreshTokenRequest(string AccessToken, string RefreshToken);
