using MassTransit;
using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Models;
using Microservice.Shared;

namespace Microservice.OrderWebAPI.Consumer;

public sealed class BasketConsumer(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<CreateOrderDto>
{
    public async Task Consume(ConsumeContext<CreateOrderDto> context)
    {
        var orders = context.Message.CreateOrderDetails.Select(s => new Order()
        {
            ProductId = s.ProductId,
            Quantity = s.Quantity,
            Status = OrderStatusEnum.Bekliyor
        }).ToList();
        dbContext.AddRange(orders);
        OrderResultDto result;
        try
        {
            await dbContext.SaveChangesAsync();
            result = new(context.Message.CreateOrderDetails, true);
        }
        catch (Exception)
        {
            result = new(context.Message.CreateOrderDetails, false);
        }

        await publishEndpoint.Publish(result);
    }
}
