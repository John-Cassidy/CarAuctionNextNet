using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(IAuctionRepository repository, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    // GetAllAuctions
    [HttpGet(Name = "GetAllAuctions")]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var auctionsDto = await _repository.GetAuctionsAsync(date);
        return Ok(auctionsDto);
    }

    // GetAuctionById
    [HttpGet("{id}", Name = "GetAuctionById")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auctionDto = await _repository.GetAuctionByIdAsync(id);
        if (auctionDto == null) return NotFound();
        return Ok(auctionDto);
    }

    // CreateAuction
    [Authorize]
    [HttpPost(Name = "CreateAuction")]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = _mapper.Map<Auction>(createAuctionDto);
        // placeholder for Seller
        auction.Seller = User.Identity.Name;

        // Using the Outbox pattern, MessageBroker messages are saved in the Outbox table
        // and are only sent to the MessageBroker after the DB transaction is committed
        // NOTE: if the DB transaction fails, the message will not be sent
        await _repository.AddAuctionAsync(auction);
        var newAuction = _mapper.Map<AuctionDto>(auction);
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
        var result = await _repository.SaveChangesAsync();

        if (!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById),
            new { id = auction.Id }, newAuction);
    }

    // UpdateAuction
    [Authorize]
    [HttpPut("{id}", Name = "UpdateAuction")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _repository.GetAuctionEntityById(id);
        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _repository.SaveChangesAsync();
        if (!result) return BadRequest("Could not save changes to the DB");
        return Ok();
    }

    // DeleteAuction
    [Authorize]
    [HttpDelete("{id}", Name = "DeleteAuction")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _repository.GetAuctionEntityById(id);
        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        _repository.RemoveAuction(auction);

        await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await _repository.SaveChangesAsync();
        if (!result) return BadRequest("Could not save changes to the DB");
        return Ok();
    }
}