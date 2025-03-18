namespace PaymentsService.Application.Exceptions;

public class ForbiddenException(string message) : Exception(message);