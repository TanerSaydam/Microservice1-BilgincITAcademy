using MassTransit;
using Microservice.Shared;

namespace Microservice.OrderWebAPI.Consumer;

public sealed class BasketConsumer : IConsumer<CreateOrderDto>
{
    public async Task Consume(ConsumeContext<CreateOrderDto> context)
    {
        await Task.CompletedTask;
    }
}
