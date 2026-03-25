namespace OrdersApp.Api.Contracts
{
    public record ModifyOrderRequest(
         string Cliente,
         string Estado,
         DateTime Fecha,
         string NumeroPedido,
         decimal Total);
}
