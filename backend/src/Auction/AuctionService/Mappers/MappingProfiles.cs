using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

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
    }
}
