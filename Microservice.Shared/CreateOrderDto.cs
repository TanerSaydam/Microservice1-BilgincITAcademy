namespace Microservice.Shared;

public sealed record CreateOrderDto(
   List<CreateOrderDetailDto> CreateOrderDetails);

public sealed record CreateOrderDetailDto(
     Guid ProductId,
     int Quantity);