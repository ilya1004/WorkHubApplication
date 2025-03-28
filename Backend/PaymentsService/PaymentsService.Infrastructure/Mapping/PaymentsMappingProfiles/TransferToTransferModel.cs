namespace PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;

public class TransferToTransferModel : Profile
{
    public TransferToTransferModel()
    {
        CreateMap<Transfer, TransferModel>();
    }
}