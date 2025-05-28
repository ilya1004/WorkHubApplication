using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;

namespace PaymentsService.API.Mapping.PaymentMappingProfiles;

public class GetOperationsRequestToGetEmployerPaymentsQuery : Profile
{
    public GetOperationsRequestToGetEmployerPaymentsQuery()
    {
        CreateMap<GetOperationsRequest, GetEmployerMyPaymentsQuery>();
    }
}