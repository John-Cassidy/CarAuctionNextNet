using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.Mappers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item).PreserveReferences();
        CreateMap<Item, AuctionDto>().PreserveReferences();
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item,
            o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionUpdated>();
        CreateMap<AuctionDto, Auction>()
            .ForMember(d => d.Status, o => o.MapFrom<StatusValueResolver>())
            .ForMember(d => d.Item,
            o => o.MapFrom(s => s));
        CreateMap<AuctionDto, Item>()
            .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Mileage))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.AuctionId, opt => opt.Ignore()) // Assuming AuctionId is not available in AuctionDto
            .ForMember(dest => dest.Auction, opt => opt.Ignore()); // Assuming direct mapping to Auction is not needed here            
    }
}
