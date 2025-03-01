namespace IdentityService.BLL.DTOs;

public record AuthTokensDTO(
    string AccessToken,
    string RefreshToken);
