using MassTransit;
using Microservice.BasketWebAPI.Context;
using Microservice.Shared;
using Microsoft.EntityFrameworkCore;

namespace Microservice.BasketWebAPI.Consumer;

public sealed class OrderResultConsumer(
    IPublishEndpoint publishEndpoint) : IConsumer<OrderResultDto>
{
    public async Task Consume(ConsumeContext<OrderResultDto> context)
    {
        if (context.Message.IsSuccessful)
        {
            ReduceProductStockDto reduceProduct = new(context.Message.Details);
            await publishEndpoint.Publish(reduceProduct);
        }
        await Task.CompletedTask;
    }
}

public sealed class ProductResultConsumer(
    ApplicationDbContext dbContext) : IConsumer<ProductResultDto>
{
    public async Task Consume(ConsumeContext<ProductResultDto> context)
    {
        if (context.Message.IsSuccessful)
        {
            var baskets = await dbContext.Baskets.ToListAsync();
            dbContext.RemoveRange(baskets);
            await dbContext.SaveChangesAsync();
        }
        await Task.CompletedTask;
    }
}
