using AutoMapper;
using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Queries;

namespace PaymentsService.API.Mapping.PaymentMappingProfiles;

public class GetEmployerPaymentsRequestToQuery : Profile
{
    public GetEmployerPaymentsRequestToQuery()
    {
        CreateMap<GetEmployerPaymentsRequest, GetEmployerPaymentsQuery>();
    }
}