namespace PaymentsService.Infrastructure.Mapping.PaymentsMappingProfiles;

public class PaymentIntentToPaymentIntentModel : Profile
{
    public PaymentIntentToPaymentIntentModel()
    {
        CreateMap<PaymentIntent, PaymentIntentModel>()
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Amount, opt => 
                opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Currency, opt => 
                opt.MapFrom(src => src.Currency))
            .ForMember(dest => dest.Status, opt => 
                opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Created, opt => 
                opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.TransferGroup, opt =>
                opt.MapFrom(src => src.TransferGroup ?? string.Empty))
            .ForMember(dest => dest.Metadata, opt => 
                opt.MapFrom(src => src.Metadata ?? new Dictionary<string, string>()));
    }
}