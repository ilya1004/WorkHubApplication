namespace IdentityService.BLL.Exceptions;

public class UnauthorizedException(string message) : Exception(message);