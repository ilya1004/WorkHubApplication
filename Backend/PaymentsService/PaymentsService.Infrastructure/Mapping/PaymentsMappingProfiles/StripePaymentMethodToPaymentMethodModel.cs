namespace PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;

public class StripePaymentMethodToPaymentMethodModel : Profile
{
    public StripePaymentMethodToPaymentMethodModel()
    {
        CreateMap<PaymentMethod, PaymentMethodModel>()
            .ForMember(dest => dest.Card, opt =>
                opt.MapFrom(src => src.Card != null
                    ? new CardModel
                    {
                        Brand = src.Card.Brand,
                        Country = src.Card.Country,
                        ExpMonth = src.Card.ExpMonth,
                        ExpYear = src.Card.ExpYear,
                        Last4Digits = src.Card.Last4
                    }
                    : null))
            .ForMember(dest => dest.CreatedAt, opt =>
                opt.MapFrom(src => src.Created));
    }
}