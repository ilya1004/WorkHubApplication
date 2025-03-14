namespace ChatService.Applications.Exceptions;

public class AlreadyExistsException(string message) : Exception(message);