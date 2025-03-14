namespace PaymentsService.Applications.Exceptions;

public class BadRequestException(string message) : Exception(message);