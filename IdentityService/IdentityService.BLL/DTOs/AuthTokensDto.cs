namespace IdentityService.BLL.DTOs;

public record AuthTokensDto(
    string AccessToken,
    string RefreshToken);