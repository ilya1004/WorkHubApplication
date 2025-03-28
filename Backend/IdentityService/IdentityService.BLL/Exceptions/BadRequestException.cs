namespace IdentityService.BLL.Exceptions;

public class BadRequestException(string message) : Exception(message);