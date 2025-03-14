namespace PaymentsService.Application.Exceptions;

public class BadRequestException(string message) : Exception(message);