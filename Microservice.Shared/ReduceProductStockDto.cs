namespace Microservice.Shared;

public sealed record ReduceProductStockDto(
    List<CreateOrderDetailDto> Details);
