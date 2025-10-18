namespace Microservice.Shared;

public sealed record OrderResultDto(
    List<CreateOrderDetailDto> Details,
    bool IsSuccessful);
