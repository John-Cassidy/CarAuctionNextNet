using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    private readonly AuctionDbContext _context;

    public AuctionCreatedFaultConsumer(AuctionDbContext context)
    {
        _context = context;
    }
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("--> Consuming faulty creation");

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == typeof(ArgumentException).FullName)
        {
            Console.WriteLine($"MassTransit:ArgumentException: {exception.Message}. Republish with different Model");

            // Republish with different Model
            var updatedModel = context.Message.Message.Model + "Bar";
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Item.Model == context.Message.Message.Model);
            if (auction != null)
            {
                auction.Item.Model = updatedModel;
                await _context.SaveChangesAsync();
            }

            context.Message.Message.Model = updatedModel;
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine($"Unhandled exception: {exception.Message}");
        }
    }
}
