namespace PaymentsService.Applications.Exceptions;

public class AlreadyExistsException(string message) : Exception(message);