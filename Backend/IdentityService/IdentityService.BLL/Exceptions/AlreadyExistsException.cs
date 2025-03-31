namespace IdentityService.BLL.Exceptions;

public class AlreadyExistsException(string message) : Exception(message);