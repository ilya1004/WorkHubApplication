namespace PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;

public class StripePaymentMethodToPaymentMethodModel : Profile
{
    public StripePaymentMethodToPaymentMethodModel()
    {
        CreateMap<PaymentMethod, PaymentMethodModel>()
            .ForMember(dest => dest.Card, opt =>
                opt.MapFrom(src => src.CardPresent != null
                    ? new CardModel
                    {
                        Brand = src.CardPresent.Brand,
                        CardholderName = src.CardPresent.CardholderName,
                        Country = src.CardPresent.Country,
                        ExpMonth = src.CardPresent.ExpMonth,
                        ExpYear = src.CardPresent.ExpYear,
                        Last4Digits = src.CardPresent.Last4
                    }
                    : null))
            .ForMember(dest => dest.CreatedAt, opt =>
                opt.MapFrom(src => src.Created));
    }
}