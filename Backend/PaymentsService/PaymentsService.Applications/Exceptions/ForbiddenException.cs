namespace PaymentsService.Applications.Exceptions;

public class ForbiddenException(string message) : Exception(message);