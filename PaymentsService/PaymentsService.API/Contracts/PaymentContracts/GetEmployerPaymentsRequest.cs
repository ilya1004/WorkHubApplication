namespace PaymentsService.API.Contracts.PaymentContracts;

public sealed record GetEmployerPaymentsRequest(
    Guid? ProjectId,
    int PageNo = 1,
    int PageSize = 10);