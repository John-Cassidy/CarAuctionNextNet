﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionRepository(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAuctionAsync(Auction auction)
    {
        await _context.Auctions.AddAsync(auction);
    }

    public async Task<AuctionDto> GetAuctionByIdAsync(Guid id)
    {
        return await _context.Auctions
           .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
           .FirstOrDefaultAsync(x => x.Id == id);

        // var auction = await _context.Auctions
        // .Include(x => x.Item)
        // .FirstOrDefaultAsync(x => x.Id == id);
        // return _mapper.Map<AuctionDto>(auction);
    }

    public async Task<Auction> GetAuctionEntityById(Guid id)
    {
        return await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<AuctionDto>> GetAuctionsAsync(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }
        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveAuction(Auction auction)
    {
        _context.Auctions.Remove(auction);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
