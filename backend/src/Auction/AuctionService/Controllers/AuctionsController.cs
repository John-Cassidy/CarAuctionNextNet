﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GetAllAuctions
    [HttpGet(Name = "GetAllAuctions")]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await _context.Auctions
        .Include(x => x.Item)
        .OrderBy(x => x.Item.Make)
        .ToListAsync();
        return Ok(_mapper.Map<List<AuctionDto>>(auctions));
    }

    // GetAuctionById
    [HttpGet("{id}", Name = "GetAuctionById")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<AuctionDto>(auction));
    }

    // CreateAuction
    [HttpPost(Name = "CreateAuction")]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = _mapper.Map<Auction>(createAuctionDto);
        // placeholder for Seller
        auction.Seller = "test";
        await _context.Auctions.AddAsync(auction);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Could not save changes to the DB");
        }
        return CreatedAtRoute(nameof(GetAuctionById),
            new { id = auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    // UpdateAuction
    [HttpPut("{id}", Name = "UpdateAuction")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }
        // TODO: Check Seller == Username

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Could not save changes to the DB");
        }
        return Ok();
    }

    // DeleteAuction
    [HttpDelete("{id}", Name = "DeleteAuction")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
        {
            return NotFound();
        }

        // TODO: Check Seller == Username

        _context.Auctions.Remove(auction);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Could not save changes to the DB");
        }
        return Ok();
    }
}