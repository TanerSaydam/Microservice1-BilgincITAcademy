using MassTransit;
using Microservice.Shared;

namespace Microservice.ProductWebAPI.Consumer;

public sealed class BasketConsumer(
    IPublishEndpoint publishEndpoint) : IConsumer<ReduceProductStockDto>
{
    public async Task Consume(ConsumeContext<ReduceProductStockDto> context)
    {
        ProductResultDto result;
        try
        {
            //stok dan düş
            result = new(true);
        }
        catch (Exception)
        {
            //başarısız sonuç oluştur
            result = new(false);
        }

        await publishEndpoint.Publish(result);
    }
}