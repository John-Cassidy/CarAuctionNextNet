using AuctionService.Controllers;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Mappers;
using AuctionService.Repositories;
using AutoFixture;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionsControllerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IAuctionRepository> _auctionRepo;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly AuctionsController _controller;
    private readonly IMapper _mapper;

    public AuctionsControllerTests()
    {
        _fixture = new Fixture();
        _auctionRepo = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfiles());
        }).CreateMapper();

        _controller = new AuctionsController(
            _auctionRepo.Object,
            _mapper,
            _publishEndpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
            }
        };
    }

    [Fact]
    public async Task GetAllAuctions_WithNoParams_ReturnsAListOfAuctions()
    {
        // Arrange
        var auctionsDto = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepo.Setup(repo => repo.GetAuctionsAsync(It.IsAny<string>()))
            .ReturnsAsync(auctionsDto);

        // Act
        var result = await _controller.GetAllAuctions(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsAssignableFrom<List<AuctionDto>>(okResult.Value);
        Assert.Equal(auctionsDto.Count, model.Count);
    }

    [Fact]
    public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
    {
        // Arrange
        var auctionDto = _fixture.Create<AuctionDto>();
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(auctionDto);

        // Act
        var result = await _controller.GetAuctionById(Guid.NewGuid());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var model = Assert.IsAssignableFrom<AuctionDto>(okResult.Value);
        Assert.Equal(auctionDto.Id, model.Id);
        Assert.Equal(auctionDto.Make, model.Make);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ReturnsNotFound()
    {
        // Arrange
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AuctionDto)null);

        // Act
        var result = await _controller.GetAuctionById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidDto_ReturnsCreatedAuction()
    {
        // Arrange
        var createAuctionDto = _fixture.Create<CreateAuctionDto>();
        var auction = _mapper.Map<Auction>(createAuctionDto);
        var auctionDto = _mapper.Map<AuctionDto>(auction);
        _auctionRepo.Setup(repo => repo.AddAuctionAsync(It.IsAny<Auction>()))
            .Returns(Task.CompletedTask);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(true);
        _publishEndpoint.Setup(p =>
            p.Publish(It.IsAny<AuctionCreated>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateAuction(createAuctionDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetAuctionById", createdAtActionResult.ActionName);
        var model = Assert.IsAssignableFrom<AuctionDto>(createdAtActionResult.Value);
        Assert.Equal(auctionDto.Id, model.Id);
        Assert.Equal(auctionDto.Make, model.Make);
    }
}
