using PaymentsService.Domain.Models;

namespace PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;

public class PaymentIntentToPaymentIntentModel : Profile
{
    public PaymentIntentToPaymentIntentModel()
    {
        CreateMap<PaymentIntent, PaymentIntentModel>()
            .ForMember(dest => dest.Amount, opt =>
                opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Currency, opt =>
                opt.MapFrom(src => src.Currency))
            .ForMember(dest => dest.TransferGroup, opt =>
                opt.MapFrom(src => src.TransferGroup));
    }
}