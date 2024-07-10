using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dbContext;

    public BidPlacedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Consuming bid placed");

        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

        // checks if the current high bid is either null 
        // or if the received bid is marked as "Accepted" and its amount is higher than the current high bid. 
        // If either condition is true, the auction's current high bid is updated to the amount of the received bid, 
        // and changes are saved to the database with SaveChangesAsync.
        if (auction.CurrentHighBid == null
            || (
                context.Message.BidStatus.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid)
            )
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _dbContext.SaveChangesAsync();
        }
    }
}
