using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerTransfers;

namespace PaymentsService.API.Mapping.PaymentMappingProfiles;

public class GetOperationsRequestToGetFreelancerTransfersQuery : Profile
{
    public GetOperationsRequestToGetFreelancerTransfersQuery()
    {
        CreateMap<GetOperationsRequest, GetFreelancerTransferQuery>();
    }
}