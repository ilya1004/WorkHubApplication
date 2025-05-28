using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;

namespace PaymentsService.API.Mapping.PaymentMappingProfiles;

public class GetOperationsRequestToGetFreelancerTransfersQuery : Profile
{
    public GetOperationsRequestToGetFreelancerTransfersQuery()
    {
        CreateMap<GetOperationsRequest, GetFreelancerMyTransfersQuery>();
    }
}