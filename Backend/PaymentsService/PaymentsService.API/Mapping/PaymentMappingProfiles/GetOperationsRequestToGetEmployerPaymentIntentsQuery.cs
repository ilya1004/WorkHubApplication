using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentIntents;

namespace PaymentsService.API.Mapping.PaymentMappingProfiles;

public class GetOperationsRequestToGetEmployerPaymentIntentsQuery : Profile
{
    public GetOperationsRequestToGetEmployerPaymentIntentsQuery()
    {
        CreateMap<GetOperationsRequest, GetEmployerPaymentIntentsQuery>();
    }
}