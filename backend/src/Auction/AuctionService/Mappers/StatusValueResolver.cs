using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Extensions;
using AutoMapper;

namespace AuctionService.Mappers;

public class StatusValueResolver : IValueResolver<AuctionDto, Auction, Status> // Assuming AuctionStatus is the enum, and Status is the correct enum type
{
    public Status Resolve(AuctionDto source, Auction destination, Status destMember, ResolutionContext context)
    {
        // Convert string to Status enum using AuctionStatus class
        return AuctionStatusExtensions.StringToStatus(source.Status);
    }
}
