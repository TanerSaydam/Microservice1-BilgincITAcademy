namespace Microservice.OrderWebAPI.Models;

public sealed class Order
{
    public Order()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public OrderStatusEnum Status { get; set; }
}

public enum OrderStatusEnum
{
    Bekliyor = 0,
    IslemTamamlandi = 1,
    Hata = 2
}