namespace PaymentsService.Application.Exceptions;

public class AlreadyExistsException(string message) : Exception(message);