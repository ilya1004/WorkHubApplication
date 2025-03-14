namespace ChatService.Applications.Exceptions;

public class ForbiddenException(string message) : Exception(message);