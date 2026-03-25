namespace OrdersApp.Api.Contracts
{
    public record CreateOrderRequest(
         string Cliente,
         string Estado,
         DateTime Fecha,
         string NumeroPedido,
         decimal Total);
}
