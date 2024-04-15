using AutoMapper;

namespace CustomerService.Models.ModelDto.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Agent, AgentReadDto>();
            CreateMap<Campaign, CampaignReadDto>();
            CreateMap<CampaignWriteDto, Campaign>();
            CreateMap<PurchaseWriteDto, Purchase>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Today))
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId))
                .ForMember(dest => dest.PriceWithDiscount,
                    opt => opt.MapFrom(src => src.Price * (100 - src.Discount) / 100))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Purchase, PurchaseReadDto>()
       .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId))
       .ForMember(dest => dest.CampaignId, opt => opt.MapFrom(src => src.CampaignId));
        }
    }
}
