namespace Microservice.BasketWebAPI.Models;

public sealed class Basket
{
    public Basket()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
