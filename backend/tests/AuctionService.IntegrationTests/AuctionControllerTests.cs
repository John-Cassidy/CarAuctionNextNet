
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

public class AuctionControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly Guid _AuctionId = new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c");

    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }
    [Fact]
    public async Task GetAuctions_ShouldReturn3Auctions()
    {
        // Arrange?

        // Act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("/api/auctions");

        // Assert
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        // Arrange?

        // Act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"/api/auctions/{_AuctionId.ToString()}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(_AuctionId, response.Id);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShouldReturn404()
    {
        // Arrange?

        // Act
        var response = await _httpClient.GetAsync($"/api/auctions/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        // Arrange
        var newAuction = GetAuctionForCreate();

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auctions", newAuction);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ShouldReturn400()
    {
        // Arrange?

        // Act
        var response = await _httpClient.GetAsync($"/api/auctions/invalidGuid");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        // Arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.NotNull(createdAuction);
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // Arrange
        var auction = new CreateAuctionDto();
        auction.Make = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/auctions", auction);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // Arrange
        var updateAuction = new UpdateAuctionDto
        {
            Make = "Ford",
            Model = "GT",
            Color = "Silver",
            Mileage = 50000,
            Year = 2020
        };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PutAsJsonAsync($"/api/auctions/{_AuctionId}", updateAuction);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // Arrange
        var updateAuction = new UpdateAuctionDto
        {
            Make = "Ford",
            Model = "GT",
            Color = "Silver",
            Mileage = 50000,
            Year = 2020
        };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("alice"));

        // Act
        var response = await _httpClient.PutAsJsonAsync($"/api/auctions/{_AuctionId}", updateAuction);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReinitDbForTests(dbContext);
        return Task.CompletedTask;
    }

    private static CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            ReservePrice = 1000000,
            AuctionEnd = DateTime.UtcNow.AddDays(10),
            Make = "Ford",
            Model = "GT",
            Color = "Silver",
            Mileage = 50000,
            Year = 2020,
            ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg"
        };
    }
}
