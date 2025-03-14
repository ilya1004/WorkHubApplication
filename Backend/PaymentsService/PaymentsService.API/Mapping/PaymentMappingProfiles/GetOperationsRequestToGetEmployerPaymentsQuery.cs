using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentsQuery;

namespace PaymentsService.API.Mapping.PaymentMappingProfiles;

public class GetOperationsRequestToGetEmployerPaymentsQuery : Profile
{
    public GetOperationsRequestToGetEmployerPaymentsQuery()
    {
        CreateMap<GetOperationsRequest, GetEmployerPaymentsQuery>();
    }
}