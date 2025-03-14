namespace ChatService.Applications.Exceptions;

public class UnauthorizedException(string message) : Exception(message);