using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using Contracts;

namespace BiddingService.Mappers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Bid, BidDto>();
        CreateMap<Bid, BidPlaced>();
    }
}