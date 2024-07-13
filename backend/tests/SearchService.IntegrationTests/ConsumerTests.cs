using AutoFixture;
using Contracts;
using MassTransit.Testing;
using MongoDB.Entities;
using SearchService.Consumers;
using SearchService.Models;

namespace SearchService.IntegrationTests;

public class ConsumerTests : IClassFixture<CustomWebAppFactory>
{
    private readonly ITestHarness _testHarness;
    private readonly Fixture _fixture;

    public ConsumerTests(CustomWebAppFactory factory)
    {
        _testHarness = factory.Services.GetTestHarness();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task AuctionCreated_ShouldCreateItemInDb()
    {
        // arrange
        var consumerHarness = _testHarness.GetConsumerHarness<AuctionCreatedConsumer>();
        var auction = _fixture.Create<AuctionCreated>();

        // act
        await _testHarness.Bus.Publish(auction);

        // assert
        Assert.True(await consumerHarness.Consumed.Any<AuctionCreated>());
        var item = await DB.Find<Item>().OneAsync(auction.Id.ToString());
        Assert.Equal(auction.Make, item.Make);
    }

    [Fact]
    public async Task AuctionUpdated_ShouldUpdateItemInDb()
    {
        // arrange
        var consumerHarness = _testHarness.GetConsumerHarness<AuctionUpdatedConsumer>();
        var auction = _fixture.Create<AuctionUpdated>();
        await new Item { ID = auction.Id.ToString() }.SaveAsync();

        // act
        await _testHarness.Bus.Publish(auction);

        // assert
        Assert.True(await consumerHarness.Consumed.Any<AuctionUpdated>());
        var item = await DB.Find<Item>().OneAsync(auction.Id.ToString());
        Assert.Equal(auction.Color, item.Color);
    }

    [Fact]
    public async Task AuctionFinished_ShouldUpdateItemInDb()
    {
        // arrange
        var consumerHarness = _testHarness.GetConsumerHarness<AuctionFinishedConsumer>();
        var auction = _fixture.Create<AuctionFinished>();
        await new Item { ID = auction.AuctionId.ToString() }.SaveAsync();

        // act
        await _testHarness.Bus.Publish(auction);

        // assert
        Assert.True(await consumerHarness.Consumed.Any<AuctionFinished>());
        var item = await DB.Find<Item>().OneAsync(auction.AuctionId.ToString());
        Assert.Equal(auction.Winner, item.Winner);
    }

    [Fact]
    public async Task AuctionDeleted_ShouldDeleteItemInDb()
    {
        // arrange
        var consumerHarness = _testHarness.GetConsumerHarness<AuctionDeletedConsumer>();
        var auction = _fixture.Create<AuctionDeleted>();
        await new Item { ID = auction.Id.ToString() }.SaveAsync();

        // act
        await _testHarness.Bus.Publish(auction);

        // assert
        Assert.True(await consumerHarness.Consumed.Any<AuctionDeleted>());
        var item = await DB.Find<Item>().OneAsync(auction.Id.ToString());
        Assert.Null(item);
    }
}