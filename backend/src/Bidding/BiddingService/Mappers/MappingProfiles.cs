using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;

namespace BiddingService.Mappers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Bid, BidDto>();
    }
}