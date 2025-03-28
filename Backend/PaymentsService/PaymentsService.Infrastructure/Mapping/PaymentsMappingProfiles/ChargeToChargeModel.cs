namespace PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;

public class ChargeToChargeModel : Profile
{
    public ChargeToChargeModel()
    {
        CreateMap<Charge, ChargeModel>();
    }
}